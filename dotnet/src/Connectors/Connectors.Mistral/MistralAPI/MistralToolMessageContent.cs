// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;
internal class MistralToolMessageContent : ChatMessageContent
{
    public MistralToolMessageContent(string functionName, string functionResult) : base(AuthorRole.Tool, functionResult)
    {
        this.FunctionName = functionName;
    }

    public string FunctionName { get; }
}
