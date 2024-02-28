// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.FunctionCalling;
public class FunctionDefinition
{
    public string Name { get;  set; }
    public string? Description { get;  set; }
    public BinaryData Parameters { get;  set; }
}
