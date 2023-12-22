// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;

    public class MistralAIEmbeddingEndpointResponse
    {
        public string id { get; set; }
        public string _object { get; set; }
        public Datum[] data { get; set; }
        public string model { get; set; }
        public Usage usage { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
        public int completion_tokens { get; set; }
    }

    public class Datum
    {
        public string _object { get; set; }
        public float[] embedding { get; set; }
        public int index { get; set; }
    }
