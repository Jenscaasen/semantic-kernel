// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Connectors.Mistral.FunctionCalling;

public class ChatCompletionsToolCall
{
    public string id { get; set; }
    public string type { get; set; }
    public ChatCompletionsToolFunctionCall function { get; set; }
}

public class ChatCompletionsToolFunctionCall
{
    public string name { get; set; }
    public string arguments { get; set; }
}
