// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;

/// <summary>
/// Represents the response body for Mistral API Embedding Endpoint.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated via JSON Deserialization")]
internal sealed class MistralAIEmbeddingEndpointResponse
{
    /// <summary>
    /// Gets or sets the ID of the response.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the object type of the response.
    /// </summary>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>
    /// Gets or sets the data of the response.
    /// </summary>
    [JsonPropertyName("data")]
    public ReadOnlyMemory<Datum> Data { get; set; }

    /// <summary>
    /// Gets or sets the model of the response.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the usage statistics for the response.
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}
/// <summary>
/// Represents the response body Usage for Mistral API Embedding Endpoint.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated via JSON Deserialization")]
internal sealed class Usage
{
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
/// Represents the response body single Embedding Result for Mistral API Embedding Endpoint.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated via JSON Deserialization")]
internal sealed class Datum
{
    /// <summary>
    /// Gets or sets the object type of the datum.
    /// </summary>
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    /// <summary>
    /// Gets or sets the embedding of the datum.
    /// </summary>
    [JsonPropertyName("embedding")]
    public ReadOnlyMemory<float> Embedding { get; set; }

    /// <summary>
    /// Gets or sets the index of the datum.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
}
