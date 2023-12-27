// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.Text;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;
/// <summary>
/// Execution settings for an Mistral completion request.
/// </summary>
public sealed class MistralPromptExecutionSettings : PromptExecutionSettings
{
    /// <summary>
    /// Temperature controls the randomness of the completion.
    /// The higher the temperature, the more random the completion.
    /// Default is 1.0.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 1;
    /// <summary>
    /// TopP controls the diversity of the completion.
    /// The higher the TopP, the more diverse the completion.
    /// Default is 1.0.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 1;
    /// <summary>
    /// The maximum number of tokens to generate in the completion.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether safe mode is enabled.
    /// </summary>
    [JsonPropertyName("safe_mode")]
    public bool SafeMode { get; set; }
    /// <summary>
    /// If specified, the system will make a best effort to sample deterministically such that repeated requests with the
    /// same seed and parameters should return the same result. Determinism is not guaranteed.
    /// </summary>
    [Experimental("SKEXP0013")]
    [JsonPropertyName("random_seed")]
    public long? Seed { get; set; }

    /// <summary>
    /// Default max tokens for a text generation
    /// </summary>
    internal static int DefaultTextMaxTokens { get; } = 256;
    /// <summary>
    /// Create a new settings object with the values from another settings object.
    /// </summary>
    /// <param name="executionSettings">Template configuration</param>
    /// <param name="defaultMaxTokens">Default max tokens</param>
    /// <returns>An instance of OpenAIPromptExecutionSettings</returns>
    public static MistralPromptExecutionSettings FromExecutionSettings(PromptExecutionSettings? executionSettings, int? defaultMaxTokens = null)
    {
        if (executionSettings is null)
        {
            return new MistralPromptExecutionSettings()
            {
                MaxTokens = defaultMaxTokens
            };
        }

        if (executionSettings is MistralPromptExecutionSettings settings)
        {
            return settings;
        }

        var json = JsonSerializer.Serialize(executionSettings);

        var openAIExecutionSettings = JsonSerializer.Deserialize<MistralPromptExecutionSettings>(json, JsonOptionsCache.ReadPermissive);
        if (openAIExecutionSettings is not null)
        {
            return openAIExecutionSettings;
        }

        throw new ArgumentException($"Invalid execution settings, cannot convert to {nameof(MistralPromptExecutionSettings)}", nameof(executionSettings));
    }
}
