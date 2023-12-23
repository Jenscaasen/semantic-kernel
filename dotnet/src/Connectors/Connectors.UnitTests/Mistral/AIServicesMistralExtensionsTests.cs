// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextGeneration;
using Xunit;

namespace SemanticKernel.Connectors.UnitTests.Mistral;

/// <summary>
/// Unit tests of <see cref="MistralServiceCollectionExtensions"/>.
/// </summary>
public class AIServicesMistralExtensionsTests
{
    [Fact]
    public void ItTellsIfAServiceIsAvailable()
    {
        Kernel targetKernel = Kernel.CreateBuilder()
            .AddMistralChatCompletion("depl", "tiny", serviceId: "mistral")
            .Build();

        // Assert
        Assert.NotNull(targetKernel.GetRequiredService<ITextGenerationService>("mistral"));
    }

    [Fact]
    public void ItCanOverwriteServices()
    {
        // Arrange
        // Act - Assert no exception occurs
        var builder = Kernel.CreateBuilder();

        builder.Services.AddMistralChatCompletion("depl", "key", serviceId: "one");
        //i dont realy get this testcase, why are we testing the service management, the openai connector already does that
        builder.Build();
    }
}
