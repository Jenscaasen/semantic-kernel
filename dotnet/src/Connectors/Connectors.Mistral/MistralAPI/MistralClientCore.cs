﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Mistral.API;
using Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;
using Microsoft.SemanticKernel.Http;

#pragma warning disable CA2208 // Instantiate argument exceptions correctly

namespace Microsoft.SemanticKernel.Connectors.Mistral;

/// <summary>
/// Base class for AI clients that provides common functionality for interacting with OpenAI services.
/// </summary>
internal class MistralClientCore
{
    internal MistralClientCore(string modelName, string apiKey, HttpClient? httpClient = null, ILogger? logger = null)
    {
        this._apiKey = apiKey;
        this._httpClient = HttpClientProvider.GetHttpClient(httpClient);
        this.DeploymentOrModelName = modelName;
        this.Logger = logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// Model Id or Deployment Name
    /// </summary>
    internal string DeploymentOrModelName { get; set; } = string.Empty;

    /// <summary>
    /// Logger instance
    /// </summary>
    internal ILogger Logger { get; set; }

    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Storage for AI service attributes.
    /// </summary>
    internal Dictionary<string, object?> Attributes { get; } = new();

    /// <summary>
    /// Creates completions for the prompt and settings.
    /// </summary>
    /// <param name="text">The prompt to complete.</param>
    /// <param name="executionSettings">Execution settings for the completion API.</param>
    /// <param name="kernel">The <see cref="Kernel"/> containing services, plugins, and other state for use throughout the operation.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Completions generated by the remote model</returns>
    internal async Task<IReadOnlyList<TextContent>> GetTextResultsAsync(
        string text,
        PromptExecutionSettings? executionSettings,
        Kernel? kernel,
        CancellationToken cancellationToken = default)
    {
        ChatHistory history = new();
        history.AddUserMessage(text);
        var chatResult = await this.GetChatMessageContentsAsync(history, executionSettings, kernel, cancellationToken).ConfigureAwait(false);
        return chatResult.Select(choice => new TextContent(choice.Content, this.DeploymentOrModelName, choice, Encoding.UTF8, choice.Metadata)).ToList();
    }

    internal IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(
        string prompt,
        PromptExecutionSettings? executionSettings,
        Kernel? kernel,
         CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Not supported by API");
    }

    /// <summary>
    /// Generate a new chat message
    /// </summary>
    /// <param name="chat">Chat history</param>
    /// <param name="executionSettings">Execution settings for the completion API.</param>
    /// <param name="kernel">The <see cref="Kernel"/> containing services, plugins, and other state for use throughout the operation.</param>
    /// <param name="cancellationToken">Async cancellation token</param>
    /// <returns>Generated chat message in string format</returns>
    internal async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chat,
        PromptExecutionSettings? executionSettings,
        Kernel? kernel,
        CancellationToken cancellationToken = default)
    {
        // Make the request.
        var responseData = await this.CallMistralChatEndpointAsync(chat).ConfigureAwait(false);

        ChatMessageContent content = new(AuthorRole.Assistant, responseData.choices[0].message.content);

        IReadOnlyDictionary<string, object?> metadata = new Dictionary<string, object?>()
        {
            {"Usage", responseData.usage }
        };
        return responseData.choices.Select(chatChoice => new ChatMessageContent(
           role: AuthorRole.Assistant,
           content: chatChoice.message.content,
            modelId: responseData.model,
            metadata: metadata
         )).ToList();
    }

    private async Task<MistralAIChatEndpointResponse> CallMistralChatEndpointAsync(ChatHistory chat)
    {
        List<Message> messages = PrepareChatMessages(chat);

        MistralAiChatEndpointRequest request = new(
            model: this.DeploymentOrModelName,
            safeMode: true,
            stream: false,
            messages: messages.ToArray()
        );
        string requestJson = JsonSerializer.Serialize(request);
        using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
        {
            string url = "https://api.mistral.ai/v1/chat/completions";
            var response = await this.CallMistralAuthedAsync(url, content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<MistralAIChatEndpointResponse>(responseContent!);
            return result!;
        }
    }

    private static List<Message> PrepareChatMessages(ChatHistory chat)
    {
        List<Message> messages = new();
        foreach (var msg in chat)
        {
            string role = "assistant";
            if (msg.Role == AuthorRole.User)
            {
                role = "user";
            }

            if (msg.Role == AuthorRole.System)
            {
                role = "system";
            }

            messages.Add(new Message(role, msg.Content));
        }

        //hack: the API does not like system-only requests, but the InvokePrompt does that by default
        if (messages.Last().role != "user")
        {
            messages.Last().role = "user";
        }

        return messages;
    }

    private async Task<MistralAIEmbeddingEndpointResponse> CallMistralEmbeddingsEndpointAsync(string[] inputs)
    {
        MistralAIEmbeddingEndpointRequest embeddingRequest = new(this.DeploymentOrModelName, inputs);

        using (var content = new StringContent(JsonSerializer.Serialize(embeddingRequest), Encoding.UTF8, "application/json"))
        {
            string url = "https://api.mistral.ai/v1/embeddings";
            var response = await this.CallMistralAuthedAsync(url, content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(responseContent))
            {
                throw new InvalidOperationException("Invalid response received from Mistral API server");
            }

            var result = JsonSerializer.Deserialize<MistralAIEmbeddingEndpointResponse>(responseContent);
            return result!;
        }
    }
    private async Task<HttpResponseMessage> CallMistralStreamingEndpointAsync(ChatHistory chat)
    {
        List<Message> messages = PrepareChatMessages(chat);

        MistralAiChatEndpointRequest request = new(
            model: this.DeploymentOrModelName,
            safeMode: true,
            stream: true,
            messages: messages.ToArray()
        );
        string requestJson = JsonSerializer.Serialize(request);
        using (var content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
        {
            string url = "https://api.mistral.ai/v1/chat/completions";
            var response = await this.CallMistralAuthedAsync(url, content).ConfigureAwait(false);
            return response;
        }
    }

    private async Task<HttpResponseMessage> CallMistralAuthedAsync(string url, StringContent content)
    {
        this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

        var response = await this._httpClient.PostAsync(url, content).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpOperationException(response.StatusCode, responseContent, response.ReasonPhrase, new HttpRequestException());
        }

        return response;
    }
    internal async IAsyncEnumerable<StreamingTextContent> GetChatAsTextStreamingContentsAsync(
       string prompt,
       PromptExecutionSettings? executionSettings,
       Kernel? kernel,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ChatHistory history = new();
        history.AddUserMessage(prompt);

        await foreach (var chatUpdate in this.GetStreamingChatMessageContentsAsync(history, executionSettings, kernel, cancellationToken))
        {
            yield return new StreamingTextContent(chatUpdate.Content, chatUpdate.ChoiceIndex, chatUpdate.ModelId, chatUpdate, Encoding.UTF8, chatUpdate.Metadata);
        }
    }

    internal async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chat,
        PromptExecutionSettings? executionSettings,
        Kernel? kernel,
       [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        StringBuilder contentBuilder = new();
        // Make the request.
        HttpResponseMessage streamingResponse = await this.CallMistralStreamingEndpointAsync(chat).ConfigureAwait(false);
        Stream responseStream = await streamingResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

        using var reader = new StreamReader(responseStream);
        bool finished = false;
        while (!reader.EndOfStream && !finished)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            if (line.StartsWith("data: ", StringComparison.InvariantCulture))
            {
                line = line.Substring(6);
            }
            var response = JsonSerializer.Deserialize<MistralAIChatStreamingResponse>(line);

            foreach (var choice in response!.choices)
            {
                contentBuilder.Append(choice.delta.content);
                var metadata = GetResponseMetadata(choice);
                yield return new MistralStreamingChatMessageContent(choice.index, this.DeploymentOrModelName, choice.delta.content, metadata);
                finished = !string.IsNullOrEmpty(choice.finishReason);
            }
        }

        // Get any response content that was streamed.
        string content = contentBuilder?.ToString() ?? string.Empty;

        chat.Add(new ChatMessageContent(AuthorRole.Assistant, content, modelId: this.DeploymentOrModelName));
    }

    private static Dictionary<string, object?> GetResponseMetadata(MistralAPI.Choice choice)
    {
        return new Dictionary<string, object?>(2)
        {
            { nameof(choice.index), choice.index },
            { nameof(choice.delta.role), choice.delta.role }
        };
    }

    internal async Task<IReadOnlyList<TextContent>> GetChatAsTextContentsAsync(
        string text,
        PromptExecutionSettings? executionSettings,
        Kernel? kernel,
        CancellationToken cancellationToken = default)
    {
        ChatHistory chat = new();
        chat.AddUserMessage(text);
        return (await this.GetChatMessageContentsAsync(chat, executionSettings, kernel, cancellationToken).ConfigureAwait(false))
            .Select(chat => new TextContent(chat.Content, chat.ModelId, chat.Content, Encoding.UTF8, chat.Metadata))
            .ToList();
    }

    internal void AddAttribute(string key, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            this.Attributes.Add(key, value);
        }
    }

    internal void LogActionDetails([CallerMemberName] string? callerMemberName = default)
    {
        if (this.Logger != null && this.Logger.IsEnabled(LogLevel.Information))
        {
            this.Logger.LogInformation("Action: {Action}. Mistral Model ID: {ModelId}.", callerMemberName, this.DeploymentOrModelName);
        }
    }

    internal async Task<IList<ReadOnlyMemory<float>>> GetEmbeddingsAsync(IList<string> dataIn, Kernel? kernel, CancellationToken cancellationToken)
    {
        MistralAIEmbeddingEndpointResponse embeddingResponse = await this.CallMistralEmbeddingsEndpointAsync(dataIn.ToArray()).ConfigureAwait(false);
        var result = new List<ReadOnlyMemory<float>>(dataIn.Count);

        foreach (var data in embeddingResponse.Data.ToArray())
        {
            result.Add(data.Embedding);
        }

        return result;
    }
}
