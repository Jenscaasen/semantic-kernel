// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;

/// <summary>
/// Represents the request body for Mistral API Embedding Endpoint.
/// </summary>
public class MistralAIEmbeddingEndpointRequest
{
    /// <summary>
    /// Gets or sets the model used for the request.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Gets or sets the input for the request.
    /// </summary>
    [JsonPropertyName("input")]
    public ReadOnlyMemory<string> Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIEmbeddingEndpointRequest"/> class.
    /// </summary>
    /// <param name="model">The model used for the request.</param>
    /// <param name="input">The input for the request.</param>
    public MistralAIEmbeddingEndpointRequest(string model, ReadOnlyMemory<string> input)
    {
        this.Model = model;
        this.Input = input;
    }
}
