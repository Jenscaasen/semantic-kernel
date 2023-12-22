// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;


    public class MistralAIEmbeddingEndpointRequest
    {
        public string model { get; set; }
        public string[] input { get; set; }
    }


