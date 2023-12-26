// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Mistral.API;
/// <summary>
/// Represents the response body from Mistral API Chat Endpoint.
/// </summary>
internal sealed class MistralAIChatEndpointResponse
{
    /// <summary>
    /// Gets the ID of the response.
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// Gets the object type of the response.
    /// </summary>
    [JsonPropertyName("object")]
    public string _object { get; set; }

    /// <summary>
    /// Gets the timestamp when the response was created.
    /// </summary>
    public int created { get; set; }

    /// <summary>
    /// Gets the model used for the response.
    /// </summary>
    public string model { get; set; }

    /// <summary>
    /// Gets the list of choices for the response.
    /// </summary>
    public Choice[] choices { get; set; }

    /// <summary>
    /// Gets the usage statistics for the response.
    /// </summary>
    public Usage usage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIChatEndpointResponse"/> class.
    /// </summary>
    /// <param name="id">The ID of the response.</param>
    /// <param name="_object">The object type of the response.</param>
    /// <param name="created">The timestamp when the response was created.</param>
    /// <param name="model">The model used for the response.</param>
    /// <param name="choices">The list of choices for the response.</param>
    /// <param name="usage">The usage statistics for the response.</param>
    [JsonConstructor]
    internal MistralAIChatEndpointResponse(string id, string _object, int created, string model, Choice[] choices, Usage usage)
    {
        this.id = id ?? string.Empty;
        this._object = _object ?? string.Empty;
        this.created = created;
        this.model = model ?? string.Empty;
        this.choices = choices ?? Array.Empty<Choice>();
        this.usage = usage;
    }
}

/// <summary>
/// Represents the usage statistics for the response.
/// </summary>
public class Usage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Usage"/> class.
    /// </summary>
    /// <param name="PromptTokens">The number of prompt tokens used.</param>
    /// <param name="TotalTokens">The total number of tokens used.</param>
    /// <param name="CompletionTokens">The number of completion tokens used.</param>
    public Usage(int PromptTokens, int TotalTokens, int CompletionTokens)
    {
        this.PromptTokens = PromptTokens;
        this.TotalTokens = TotalTokens;
        this.CompletionTokens = CompletionTokens;
    }
    /// <summary>
    /// Gets the number of prompt tokens used.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets the total number of tokens used.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    /// <summary>
    /// Gets the number of completion tokens used.
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
}

/// <summary>
/// Represents a choice for the response.
/// </summary>
public class Choice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Choice"/> class.
    /// </summary>
    /// <param name="index">The index of the choice.</param>
    /// <param name="message">The message for the choice.</param>
    /// <param name="FinnishReason">The reason why the choice was finished.</param>
    [JsonConstructor]
    public Choice(int index, Message message, string FinnishReason)
    {
        this.index = index;
        this.message = message;
        this.FinnishReason = FinnishReason;
    }

    /// <summary>
    /// Gets the index of the choice.
    /// </summary>
    public int index { get; set; }

    /// <summary>
    /// Gets the message for the choice.
    /// </summary>
    public Message message { get; set; }

    /// <summary>
    /// Gets the reason why the choice was finished.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string FinnishReason { get; set; }
}

/// <summary>
/// Represents a message for a choice.
/// </summary>
public class Message
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    /// <param name="role">The role of the message.</param>
    /// <param name="content">The content of the message.</param>
    [JsonConstructor]
    public Message(string role, string? content)
    {
        this.role = role ?? string.Empty;
        this.content = content ?? string.Empty;
    }
    /// <summary>
    /// Gets the role of the message.
    /// </summary>
    public string role { get; set; }

    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    public string content { get; set; }
}
