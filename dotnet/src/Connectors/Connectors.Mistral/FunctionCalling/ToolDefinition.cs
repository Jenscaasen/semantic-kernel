// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Connectors.Mistral.FunctionCalling;
public class ToolDefinition
{
    public string type { get; set; }
    public FunctionDefinition function { get; set; }
}
