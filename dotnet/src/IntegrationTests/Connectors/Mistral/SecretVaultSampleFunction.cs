// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel.IntegrationTests.Connectors.Mistral;

public class SecretVaultSampleFunction
{
    [KernelFunction, Description("Returns a secret from the secret vault")]
    public string GetSecretFromVault([Description("The id of the secret")] int secretId)
    {
        if (secretId == 3)
        {
            return "Known as the founder of the Impressionism movement, Claude Monet’s work is recognized worldwide.";
        }

        return "No secret found for id " + secretId;
    }
}
