// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Microsoft.SemanticKernel.Connectors.Mistral;

/// <summary>
/// Azure Mistral and Mistral Specialized streaming chat message content.
/// </summary>
/// <remarks>
/// Represents a chat message content chunk that was streamed from the remote model.
/// </remarks>
public sealed class MistralStreamingChatMessageContent : StreamingChatMessageContent
{
    /// <summary>
    /// Create a new instance of the <see cref="MistralStreamingChatMessageContent"/> class.
    /// </summary>
    /// <param name="choiceIndex">Index of the choice</param>
    /// <param name="modelId">The model ID used to generate the content</param>
    /// <param name="content">Content of the message</param>
    /// /// <param name="metadata">Additional metadata</param>
    internal MistralStreamingChatMessageContent(
        int choiceIndex,
        string modelId,
        string content,
        IReadOnlyDictionary<string, object?>? metadata = null)
        : base(
            AuthorRole.Assistant,
            content,
           choiceIndex: choiceIndex,
          modelId: modelId,
          encoding: Encoding.UTF8,
          metadata: metadata)
    {
    }

    /// <inheritdoc/>
    public override byte[] ToByteArray() => this.Encoding.GetBytes(this.ToString());

    /// <inheritdoc/>
    public override string ToString() => this.Content ?? string.Empty;
}
