// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Connectors.Mistral.FunctionCalling;
using Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;

namespace Microsoft.SemanticKernel.Connectors.Mistral.API;

/// <summary>
/// Represents the request body for Mistral AI Chat Endpoint.
/// </summary>
public class MistralAiChatEndpointRequest
{
    /// <summary>
    /// Gets or sets if streaming should be enabled
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
    /// <summary>
    /// Gets or sets the model for the chat request.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the messages for the chat request.
    /// </summary>
    [JsonPropertyName("messages")]
    public ReadOnlyMemory<Message> Messages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether safe prompt is enabled.
    /// </summary>
    [JsonPropertyName("safe_prompt")]
    public bool SafePrompt { get; set; }
    /// <summary>
    /// Temperature controls the randomness of the completion.
    /// The higher the temperature, the more random the completion.
    /// Default is 1.0.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 1;
    /// <summary>
    /// The maximum number of tokens to generate in the completion.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }
    [JsonPropertyName("tools")]
    public List<ToolDefinition> Tools { get; set; }

    /// <summary>
    /// TopP controls the diversity of the completion.
    /// The higher the TopP, the more diverse the completion.
    /// Default is 1.0.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 1;
    /// <summary>
    /// If specified, the system will make a best effort to sample deterministically such that repeated requests with the
    /// same seed and parameters should return the same result. Determinism is not guaranteed.
    /// </summary>
    [Experimental("SKEXP0013")]
    [JsonPropertyName("random_seed")]
    public long? Seed { get; set; }
    internal void ApplySettings(MistralPromptExecutionSettings textExecutionSettings)
    {
        this.SafePrompt = textExecutionSettings.SafePrompt;
        this.Temperature = this.Clamp(textExecutionSettings.Temperature, 0.1, 1);
        this.TopP = this.Clamp(textExecutionSettings.TopP, 0.1, 1);
        this.SafePrompt = textExecutionSettings.SafePrompt;
        this.Seed = textExecutionSettings.Seed;
        this.MaxTokens = textExecutionSettings.MaxTokens ?? MistralPromptExecutionSettings.DefaultTextMaxTokens; //otherwise the endpoint crashes at the moment

        if (textExecutionSettings.Tools != null)
        {
            this.Tools = textExecutionSettings.Tools.Select(t => new ToolDefinition { type = "function", function = t }).ToList();
        } else
        {
            this.Tools = new List<ToolDefinition>();
        }
    }

    private double Clamp(double value, double min, double max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
}
