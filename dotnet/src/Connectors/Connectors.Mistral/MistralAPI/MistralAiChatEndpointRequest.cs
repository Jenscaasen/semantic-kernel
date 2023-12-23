﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.API;

/// <summary>
/// Represents the request body for Mistral AI Chat Endpoint.
/// </summary>
public class MistralAiChatEndpointRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAiChatEndpointRequest"/> class.
    /// </summary>
    /// <param name="model">The model for the chat request.</param>
    /// <param name="messages">The messages for the chat request.</param>
    /// <param name="safeMode">Indicates if safe mode is enabled.</param>
    public MistralAiChatEndpointRequest(string model, Message[] messages, bool safeMode)
    {
        this.Model = model;
        this.Messages = messages;
        this.SafeMode = safeMode;
    }

    /// <summary>
    /// Gets or sets the model for the chat request.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    /// Gets or sets the messages for the chat request.
    /// </summary>
    [JsonConverter(typeof(ReadOnlyMemoryConverter))]
    public ReadOnlyMemory<Message> Messages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether safe mode is enabled.
    /// </summary>
    public bool SafeMode { get; set; }
}
