// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;
/// <summary>
/// Represents the response body from Mistral API Chat Streaming Endpoint.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated via JSON Deserialization")]
internal sealed class MistralAIChatStreamingResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIChatStreamingResponse"/> class.
    /// </summary>
    /// <param name="id">The ID of the response.</param>
    /// <param name="_object">The object type of the response.</param>
    /// <param name="created">The timestamp when the response was created.</param>
    /// <param name="model">The model used for the response.</param>
    /// <param name="choices">The list of choices for the response.</param>
    [JsonConstructor]
    internal MistralAIChatStreamingResponse(string id, string _object, int created, string model, Choice[] choices)
    {
        this.id = id ?? string.Empty;
        this._object = _object ?? string.Empty;
        this.created = created;
        this.model = model ?? string.Empty;
        this.choices = choices ?? Array.Empty<Choice>();
    }

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
}
/// <summary>
/// Represents a choice for a streaming response
/// </summary>
public class Choice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Choice"/> class.
    /// </summary>
    /// <param name="index">The index of the choice.</param>
    /// <param name="delta">The delta of the choice.</param>
    /// <param name="finishReason">The finish reason of the choice.</param>
    [JsonConstructor]
    public Choice(int index, Delta delta, string finishReason)
    {
        this.index = index;
        this.delta = delta;
        this.finishReason = finishReason;
    }

    /// <summary>
    /// Gets or sets the index of the choice.
    /// </summary>
    public int index { get; set; }

    /// <summary>
    /// Gets or sets the delta of the choice.
    /// </summary>
    public Delta delta { get; set; }

    /// <summary>
    /// Gets or sets the finish reason of the choice.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string finishReason { get; set; }
}
/// <summary>
/// Represents a role and content pair in a streaming message
/// </summary>
public class Delta
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Delta"/> class.
    /// </summary>
    /// <param name="role">The role of the delta.</param>
    /// <param name="content">The content of the delta.</param>
    [JsonConstructor]
    public Delta(string role, string content)
    {
        this.role = role;
        this.content = content ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the role of the delta.
    /// </summary>
    public string role { get; set; }

    /// <summary>
    /// Gets or sets the content of the delta.
    /// </summary>
    public string content { get; set; }
}
