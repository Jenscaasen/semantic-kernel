// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;

namespace Microsoft.SemanticKernel.Connectors.Mistral;

/// <summary>
/// Mistral chat completion service.
/// </summary>
public sealed class MistralAIChatCompletionService : IChatCompletionService, ITextGenerationService
{
    private readonly MistralClientCore _core;

    /// <summary>
    /// Create an instance of the Mistral chat completion connector
    /// </summary>
    /// <param name="modelId">Model name</param>
    /// <param name="apiKey">Mistral API Key</param>
    /// <param name="httpClient">Custom <see cref="HttpClient"/> for HTTP requests.</param>
    /// <param name="endpoint">Custom endpoint for the Mistral service.</param>
    public MistralAIChatCompletionService(
        string modelId,
        string apiKey,
        HttpClient? httpClient = null,
        string? endpoint = null)
    {
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        this._core = new MistralClientCore(modelId, apiKey, endpoint, httpClient);

        this._core.AddAttribute(AIServiceExtensions.ModelIdKey, modelId);
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object?> Attributes => this._core.Attributes;

    /// <inheritdoc/>
    public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => this._core.GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => this._core.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => this._core.GetChatAsTextContentsAsync(prompt, executionSettings, kernel, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        => this._core.GetChatAsTextStreamingContentsAsync(prompt, executionSettings, kernel, cancellationToken);
}
