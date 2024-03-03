// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.FunctionCalling;
public class ToolDefinition
{
    public string type { get; set; }
    public FunctionDefinition function { get; set; }
}
