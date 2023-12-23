// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;

/// <summary>
/// Represents the response body for Mistral API Embedding Endpoint.
/// </summary>
public class MistralAIEmbeddingEndpointResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIEmbeddingEndpointResponse"/> class.
    /// </summary>
    /// <param name="Id">The ID of the response.</param>
    /// <param name="ObjAsText">The object type of the response.</param>
    /// <param name="Data">The data of the response.</param>
    /// <param name="Model">The model of the response.</param>
    /// <param name="Usage">The usage statistics for the response.</param>
    public MistralAIEmbeddingEndpointResponse(string Id, string ObjAsText, Datum[] Data, string Model, Usage Usage)
    {
        this.Id = Id;
        this.ObjAsText = ObjAsText;
        this.Data = Data;
        this.Model = Model;
        this.Usage = Usage;
    }

    /// <summary>
    /// Gets or sets the ID of the response.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the object type of the response.
    /// </summary>
    [JsonPropertyName("Object")]
    public string ObjAsText { get; set; }

    /// <summary>
    /// Gets or sets the data of the response.
    /// </summary>
    [JsonConverter(typeof(ReadOnlyMemoryConverter))]
    public ReadOnlyMemory<Datum> Data { get; set; }

    /// <summary>
    /// Gets or sets the model of the response.
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Gets or sets the usage statistics for the response.
    /// </summary>
    public Usage Usage { get; set; }
}
/// <summary>
/// Represents the response body Usage for Mistral API Embedding Endpoint.
/// </summary>
public class Usage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Usage"/> class.
    /// </summary>
    /// <param name="promptTokens">The number of prompt tokens used.</param>
    /// <param name="totalTokens">The total number of tokens used.</param>
    /// <param name="completionTokens">The number of completion tokens used.</param>
    public Usage(int promptTokens, int totalTokens, int completionTokens)
    {
        this.PromptTokens = promptTokens;
        this.TotalTokens = totalTokens;
        this.CompletionTokens = completionTokens;
    }

    /// <summary>
    /// Gets or sets the number of prompt tokens used.
    /// </summary>
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens used.
    /// </summary>
    public int TotalTokens { get; set; }

    /// <summary>
    /// Gets or sets the number of completion tokens used.
    /// </summary>
    public int CompletionTokens { get; set; }
}
/// <summary>
/// Represents the response body single Embedding Result for Mistral API Embedding Endpoint.
/// </summary>
public class Datum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Datum"/> class.
    /// </summary>
    /// <param name="type">The object type of the datum.</param>
    /// <param name="embedding">The embedding of the datum.</param>
    /// <param name="index">The index of the datum.</param>
    public Datum(string type, float[] embedding, int index)
    {
        this.type = type;
        this.Embedding = embedding;
        this.Index = index;
    }

    /// <summary>
    /// Gets or sets the object type of the datum.
    /// </summary>
    [JsonPropertyName("object")]
    public string type { get; set; }

    /// <summary>
    /// Gets or sets the embedding of the datum.
    /// </summary>
    [JsonConverter(typeof(ReadOnlyMemoryConverter))]
    public ReadOnlyMemory<float> Embedding { get; set; }

    /// <summary>
    /// Gets or sets the index of the datum.
    /// </summary>
    public int Index { get; set; }
}
