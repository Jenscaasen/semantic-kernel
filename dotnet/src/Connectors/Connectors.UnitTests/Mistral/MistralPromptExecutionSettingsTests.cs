// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Mistral.MistralAPI;
using Xunit;

namespace SemanticKernel.Connectors.UnitTests.Mistral;

/// <summary>
/// Unit tests of MistralPromptExecutionSettings
/// </summary>
public class MistralPromptExecutionSettingsTests
{
    [Fact]
    public void ItCreatesMistralExecutionSettingsWithCorrectDefaults()
    {
        // Arrange
        // Act
        MistralPromptExecutionSettings executionSettings = MistralPromptExecutionSettings.FromExecutionSettings(null, 256);

        // Assert
        Assert.NotNull(executionSettings);
        Assert.Equal(1, executionSettings.Temperature);
        Assert.Equal(1, executionSettings.TopP);
        Assert.Equal(256, executionSettings.MaxTokens);
    }

    [Fact]
    public void ItUsesExistingMistralExecutionSettings()
    {
        // Arrange
        MistralPromptExecutionSettings actualSettings = new()
        {
            Temperature = 0.7,
            TopP = 0.7,
            MaxTokens = 128,
        };

        // Act
        MistralPromptExecutionSettings executionSettings = MistralPromptExecutionSettings.FromExecutionSettings(actualSettings);

        // Assert
        Assert.NotNull(executionSettings);
        Assert.Equal(actualSettings, executionSettings);
    }

    [Fact]
    public void ItCanUseMistralExecutionSettings()
    {
        // Arrange
        PromptExecutionSettings actualSettings = new()
        {
            ExtensionData = new() {
                { "max_tokens", 1000 },
                { "temperature", 0 }
            }
        };

        // Act
        MistralPromptExecutionSettings executionSettings = MistralPromptExecutionSettings.FromExecutionSettings(actualSettings, null);

        // Assert
        Assert.NotNull(executionSettings);
        Assert.Equal(1000, executionSettings.MaxTokens);
        Assert.Equal(0, executionSettings.Temperature);
    }

    [Fact]
    public void ItCreatesMistralExecutionSettingsFromExtraPropertiesSnakeCase()
    {
        // Arrange
        PromptExecutionSettings actualSettings = new()
        {
            ExtensionData = new Dictionary<string, object>()
            {
                { "temperature", 0.7 },
                { "top_p", 0.7 },
                { "max_tokens", 128 },
                { "random_seed", 123456 },
            }
        };

        // Act
        MistralPromptExecutionSettings executionSettings = MistralPromptExecutionSettings.FromExecutionSettings(actualSettings, null);

        // Assert
        AssertExecutionSettings(executionSettings);
        Assert.Equal(executionSettings.Seed, 123456);
    }

    [Fact]
    public void ItCreatesMistralExecutionSettingsFromExtraPropertiesAsStrings()
    {
        // Arrange
        PromptExecutionSettings actualSettings = new()
        {
            ExtensionData = new Dictionary<string, object>()
            {
                { "temperature", "0.7" },
                { "top_p", "0.7" },
                { "max_tokens", "128" },
          }
        };

        // Act
        MistralPromptExecutionSettings executionSettings = MistralPromptExecutionSettings.FromExecutionSettings(actualSettings, null);

        // Assert
        AssertExecutionSettings(executionSettings);
    }

    [Fact]
    public void ItCreatesMistralExecutionSettingsFromJsonSnakeCase()
    {
        // Arrange
        var json = @"{
  ""temperature"": 0.7,
  ""top_p"": 0.7,
  ""max_tokens"": 128
}";
        var actualSettings = JsonSerializer.Deserialize<PromptExecutionSettings>(json);

        // Act
        MistralPromptExecutionSettings executionSettings = MistralPromptExecutionSettings.FromExecutionSettings(actualSettings);

        // Assert
        AssertExecutionSettings(executionSettings);
    }

    private static void AssertExecutionSettings(MistralPromptExecutionSettings executionSettings)
    {
        Assert.NotNull(executionSettings);
        Assert.Equal(0.7, executionSettings.Temperature);
        Assert.Equal(0.7, executionSettings.TopP);
        Assert.Equal(128, executionSettings.MaxTokens);
    }
}
