// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.Mistral;
using Microsoft.SemanticKernel.Embeddings;
using SemanticKernel.IntegrationTests.TestSettings;
using Xunit;
using Xunit.Abstractions;

namespace SemanticKernel.IntegrationTests.Connectors.Mistral;

public sealed class MistralTextEmbeddingTests : IDisposable
{
    private const int MistralVectorLength = 1024;
    private readonly IConfigurationRoot _configuration;

    public MistralTextEmbeddingTests(ITestOutputHelper output)
    {
        this._testOutputHelper = new RedirectOutput(output);
        Console.SetOut(this._testOutputHelper);

        // Load configuration
        this._configuration = new ConfigurationBuilder()
            .AddJsonFile(path: "testsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(path: "testsettings.development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<MistralTextEmbeddingTests>()
            .Build();
    }

    [Theory]
    [InlineData("test sentence")]
    public async Task MistralEmbeddingTestAsync(string testInputString)
    {
        // Arrange
        MistralConfiguration? MistralConfiguration = this._configuration.GetSection("MistralEmbeddings").Get<MistralConfiguration>();
        Assert.NotNull(MistralConfiguration);

        var embeddingGenerator = new MistralTextEmbeddingGenerationService(MistralConfiguration.ModelId, MistralConfiguration.ApiKey);

        // Act
        var singleResult = await embeddingGenerator.GenerateEmbeddingAsync(testInputString);
        var batchResult = await embeddingGenerator.GenerateEmbeddingsAsync(new List<string> { testInputString, testInputString, testInputString });

        // Assert
        Assert.Equal(MistralVectorLength, singleResult.Length);
        Assert.Equal(3, batchResult.Count);
    }


    #region internals

    private readonly RedirectOutput _testOutputHelper;

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MistralTextEmbeddingTests()
    {
        this.Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._testOutputHelper.Dispose();
        }
    }

    #endregion
}
