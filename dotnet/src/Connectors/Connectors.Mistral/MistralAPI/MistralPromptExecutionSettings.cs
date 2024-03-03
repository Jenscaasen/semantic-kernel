// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Connectors.Mistral.FunctionCalling;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;
/// <summary>
/// Execution settings for an Mistral completion request.
/// </summary>
[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
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
    [JsonPropertyName("safe_prompt")]
    public bool SafePrompt { get; set; }
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
    internal static int DefaultTextMaxTokens { get; } = 4192;

    /// <summary>
    /// Gets or sets the behavior for how tool calls are handled.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>To disable all tool calling, set the property to null (the default).</item>
    /// <item>
    /// To request that the model use a specific function, set the property to an instance returned
    /// from <see cref="ToolCallBehavior.RequireFunction"/>.
    /// </item>
    /// <item>
    /// To allow the model to request one of any number of functions, set the property to an
    /// instance returned from <see cref="ToolCallBehavior.EnableFunctions"/>, called with
    /// a list of the functions available.
    /// </item>
    /// <item>
    /// To allow the model to request one of any of the functions in the supplied <see cref="Kernel"/>,
    /// set the property to <see cref="ToolCallBehavior.EnableKernelFunctions"/> if the client should simply
    /// send the information about the functions and not handle the response in any special manner, or
    /// <see cref="ToolCallBehavior.AutoInvokeKernelFunctions"/> if the client should attempt to automatically
    /// invoke the function and send the result back to the service.
    /// </item>
    /// </list>
    /// For all options where an instance is provided, auto-invoke behavior may be selected. If the service
    /// sends a request for a function call, if auto-invoke has been requested, the client will attempt to
    /// resolve that function from the functions available in the <see cref="Kernel"/>, and if found, rather
    /// than returning the response back to the caller, it will handle the request automatically, invoking
    /// the function, and sending back the result. The intermediate messages will be retained in the
    /// <see cref="ChatHistory"/> if an instance was provided.
    /// </remarks>
    public ToolCallBehavior? ToolCallBehavior
    {
        get => this._toolCallBehavior;

        set
        {
            this.ThrowIfFrozen();
            this._toolCallBehavior = value;
        }
    }

    public ChatCompletionsToolChoice ToolChoice { get; set; }
    /// <summary>
    /// The list of tools to invoke. If a kernel is provided with plugins, this will be populated automatically
    /// </summary>
    public List<FunctionDefinition> Tools { get; set; }

    /// <summary>
    /// Create a new settings object with the values from another settings object.
    /// </summary>
    /// <param name="executionSettings">Template configuration</param>
    /// <param name="defaultMaxTokens">Default max tokens</param>
    /// <returns>An instance of MistralPromptExecutionSettings</returns>
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

        var mistralExecutionSettings = JsonSerializer.Deserialize<MistralPromptExecutionSettings>(json, JsonOptionsCache.ReadPermissive);
        if (mistralExecutionSettings is not null)
        {
            return mistralExecutionSettings;
        }

        throw new ArgumentException($"Invalid execution settings, cannot convert to {nameof(MistralPromptExecutionSettings)}", nameof(executionSettings));
    }
    private ToolCallBehavior? _toolCallBehavior;
}
