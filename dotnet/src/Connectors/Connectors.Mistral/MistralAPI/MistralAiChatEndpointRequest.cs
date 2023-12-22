// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.AzureSdk;

public class MistralAiChatEndpointRequest
{
    public string model { get; set; }
    public Message[] messages { get; set; }
    public bool safe_mode { get; set; }
}
