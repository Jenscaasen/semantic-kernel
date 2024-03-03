// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Mistral.API;

namespace Microsoft.SemanticKernel.Connectors.Mistral;

/// <summary>
/// Mistral specialized chat message content
/// </summary>
public sealed class MistralAssitantMessageContent : ChatMessageContent
{
    /// <summary>
    /// Gets the metadata key for the list of <see cref="ChatCompletionsFunctionToolCall"/>.
    /// </summary>
    internal static string FunctionToolCallsProperty => $"{nameof(MistralAIChatEndpointResponse)}.FunctionToolCalls";

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAssitantMessageContent"/> class.
    /// </summary>
    /// <param name="chatMessage">chat message</param>
    /// <param name="metadata">Additional metadata</param>
    internal MistralAssitantMessageContent(AuthorRole role, MistralAIChatEndpointResponse chatMessage, IReadOnlyDictionary<string, object?>? metadata = null)
        : base(role, chatMessage.choices[0].message.content, chatMessage.model, chatMessage, System.Text.Encoding.UTF8, CreateMetadataDictionary(chatMessage.choices[0].message.tool_calls, metadata))
    {
        this.ToolCalls = chatMessage.choices[0].message.tool_calls;
    }

    /// <summary>
    /// A list of the tools called by the model.
    /// </summary>
    public IReadOnlyList<tool_call> ToolCalls { get; }

    /// <summary>
    /// Retrieve the resulting function from the chat result.
    /// </summary>
    /// <returns>The <see cref="tool_call"/>, or null if no function was returned by the model.</returns>
    public IReadOnlyList<tool_call> GetMistralFunctionToolCalls()
    {
        List<tool_call>? functionToolCallList = null;

        foreach (var toolCall in this.ToolCalls)
        {
            if (toolCall is tool_call functionToolCall)
            {
                (functionToolCallList ??= new List<tool_call>()).Add(functionToolCall);
            }
        }

        if (functionToolCallList is not null)
        {
            return functionToolCallList;
        }

        return Array.Empty<tool_call>();
    }

    private static IReadOnlyDictionary<string, object?>? CreateMetadataDictionary(
        IReadOnlyList<tool_call> toolCalls,
        IReadOnlyDictionary<string, object?>? original)
    {
        // We only need to augment the metadata if there are any tool calls.
        if (toolCalls.Count > 0)
        {
            Dictionary<string, object?> newDictionary;
            if (original is null)
            {
                // There's no existing metadata to clone; just allocate a new dictionary.
                newDictionary = new Dictionary<string, object?>(1);
            }
            else if (original is IDictionary<string, object?> origIDictionary)
            {
                // Efficiently clone the old dictionary to a new one.
                newDictionary = new Dictionary<string, object?>(origIDictionary);
            }
            else
            {
                // There's metadata to clone but we have to do so one item at a time.
                newDictionary = new Dictionary<string, object?>(original.Count + 1);
                foreach (var kvp in original)
                {
                    newDictionary[kvp.Key] = kvp.Value;
                }
            }

            // Add the additional entry.
            newDictionary.Add(FunctionToolCallsProperty, toolCalls.OfType<tool_call>().ToList());

            return newDictionary;
        }

        return original;
    }
}
