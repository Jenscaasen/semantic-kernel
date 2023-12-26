// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernel.IntegrationTests.TestSettings;
using Xunit;
using Xunit.Abstractions;

namespace SemanticKernel.IntegrationTests.Connectors.Mistral;

#pragma warning disable xUnit1004 // Contains test methods used in manual verification. Disable warning for this file only.

public sealed class MistralCompletionTests : IDisposable
{
    private const string InputParameterName = "input";
    private readonly IKernelBuilder _kernelBuilder;
    private readonly IConfigurationRoot _configuration;

    public MistralCompletionTests(ITestOutputHelper output)
    {
        this._logger = new XunitLogger<Kernel>(output);
        this._testOutputHelper = new RedirectOutput(output);
        Console.SetOut(this._testOutputHelper);

        // Load configuration
        this._configuration = new ConfigurationBuilder()
            .AddJsonFile(path: "testsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(path: "testsettings.development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<MistralCompletionTests>()
            .Build();

        this._kernelBuilder = Kernel.CreateBuilder();
    }

    [Theory]
    [InlineData("Where is the most famous fish market in Seattle, Washington, USA?", "Pike Place Market")]
    public async Task MistralTestAsync(string prompt, string expectedAnswerContains)
    {
        // Arrange
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();
        Assert.NotNull(mistralConfiguration);

        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        Kernel target = this._kernelBuilder
            .AddMistralChatCompletion(
                serviceId: mistralConfiguration.ServiceId,
                modelId: mistralConfiguration.ModelId,
                apiKey: mistralConfiguration.ApiKey)
            .Build();

        IReadOnlyKernelPluginCollection plugins = TestHelpers.ImportSamplePlugins(target, "ChatPlugin");

        // Act
        FunctionResult actual = await target.InvokeAsync(plugins["ChatPlugin"]["Chat"], new() { [InputParameterName] = prompt });

        // Assert
        Assert.Contains(expectedAnswerContains, actual.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Where is the most famous fish market in Seattle, Washington, USA?", "Pike Place Market")]
    public async Task MistralChatAsTextTestAsync(string prompt, string expectedAnswerContains)
    {
        // Arrange
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        IKernelBuilder builder = this._kernelBuilder;

        this.ConfigureChatMistral(builder);

        Kernel target = builder.Build();

        IReadOnlyKernelPluginCollection plugins = TestHelpers.ImportSamplePlugins(target, "ChatPlugin");

        // Act
        FunctionResult actual = await target.InvokeAsync(plugins["ChatPlugin"]["Chat"], new() { [InputParameterName] = prompt });

        // Assert
        Assert.Contains(expectedAnswerContains, actual.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(false, "Where is the most famous fish market in Seattle, Washington, USA?", "Pike Place")]
    [InlineData(true, "Where is the most famous fish market in Seattle, Washington, USA?", "Pike Place")]
    public async Task MistralStreamingTestAsync(bool useChatModel, string prompt, string expectedAnswerContains)
    {
        // Arrange
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        var builder = this._kernelBuilder;

        if (useChatModel)
        {
            this.ConfigureMistralChatAsText(builder);
        }
        else
        {
            this.ConfigureMistralText(builder);
        }

        Kernel target = builder.Build();

        IReadOnlyKernelPluginCollection plugins = TestHelpers.ImportSamplePlugins(target, "ChatPlugin");

        StringBuilder fullResult = new();
        // Act
        await foreach (var content in target.InvokeStreamingAsync<StreamingKernelContent>(plugins["ChatPlugin"]["Chat"], new() { [InputParameterName] = prompt }))
        {
            fullResult.Append(content);
        };

        // Assert
        Assert.Contains(expectedAnswerContains, fullResult.ToString(), StringComparison.OrdinalIgnoreCase);
    }
    [Fact(Skip = "Skipping while we investigate issue with GitHub actions.")]
    public async Task CanUseMistralChatForTextGenerationAsync()
    {
        // Note: we use OpenAI Chat Completion and GPT 3.5 Turbo
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        IKernelBuilder builder = this._kernelBuilder;
        this.ConfigureChatMistral(builder);

        Kernel target = builder.Build();

        var func = target.CreateFunctionFromPrompt(
            "List the two planets after '{{$input}}', excluding moons, using bullet points.");

        var result = await func.InvokeAsync(target, new() { [InputParameterName] = "Jupiter" });

        Assert.NotNull(result);
        Assert.Contains("Saturn", result.GetValue<string>(), StringComparison.InvariantCultureIgnoreCase);
        Assert.Contains("Uranus", result.GetValue<string>(), StringComparison.InvariantCultureIgnoreCase);
    }

    // If the test fails, please note that SK retry logic may not be fully integrated into the underlying code using Azure SDK
    [Theory]
    [InlineData("Where is the most famous fish market in Seattle, Washington, USA?", "Resilience event occurred")]
    public async Task MistralHttpRetryPolicyTestAsync(string prompt, string expectedOutput)
    {
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();
        Assert.NotNull(mistralConfiguration);

        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._testOutputHelper);
        this._kernelBuilder
            .AddMistralTextCompletion(
                serviceId: mistralConfiguration.ServiceId,
                modelId: mistralConfiguration.ModelId,
                apiKey: "INVALID_KEY"); // Use an invalid API key to force a 401 Unauthorized response
        this._kernelBuilder.Services.ConfigureHttpClientDefaults(c =>
        {
            // Use a standard resiliency policy, augmented to retry on 401 Unauthorized for this example
            c.AddStandardResilienceHandler().Configure(o =>
            {
                o.Retry.ShouldHandle = args => ValueTask.FromResult(args.Outcome.Result?.StatusCode is HttpStatusCode.Unauthorized);
            });
        });
        Kernel target = this._kernelBuilder.Build();

        IReadOnlyKernelPluginCollection plugins = TestHelpers.ImportSamplePlugins(target, "SummarizePlugin");

        // Act
        await Assert.ThrowsAsync<HttpOperationException>(() => target.InvokeAsync(plugins["SummarizePlugin"]["Summarize"], new() { [InputParameterName] = prompt }));

        // Assert
        Assert.Contains(expectedOutput, this._testOutputHelper.GetLogs(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MistralShouldReturnTokenUsageInMetadataAsync(bool useChatModel)
    {
        // Arrange
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        var builder = this._kernelBuilder;

        if (useChatModel)
        {
            this.ConfigureMistralChatAsText(builder);
        }
        else
        {
            this.ConfigureMistralText(builder);
        }

        Kernel target = builder.Build();

        IReadOnlyKernelPluginCollection plugin = TestHelpers.ImportSamplePlugins(target, "FunPlugin");

        // Act and Assert
        FunctionResult result = await target.InvokeAsync(plugin["FunPlugin"]["Limerick"]);

        Assert.NotNull(result.Metadata);
        Assert.True(result.Metadata.TryGetValue("Usage", out object? usageObject));
        Assert.NotNull(usageObject);

        var jsonObject = JsonSerializer.SerializeToElement(usageObject);
        Assert.True(jsonObject.TryGetProperty("prompt_tokens", out JsonElement promptTokensJson));
        Assert.True(promptTokensJson.TryGetInt32(out int promptTokens));
        Assert.NotEqual(0, promptTokens);
        Assert.True(jsonObject.TryGetProperty("completion_tokens", out JsonElement completionTokensJson));
        Assert.True(completionTokensJson.TryGetInt32(out int completionTokens));
        Assert.NotEqual(0, completionTokens);
    }

    [Fact]
    public async Task MistralHttpInvalidKeyShouldReturnErrorDetailAsync()
    {
        // Arrange
        MistralConfiguration? mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();
        Assert.NotNull(mistralConfiguration);

        // Use an invalid API key to force a 401 Unauthorized response
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        Kernel target = this._kernelBuilder
            .AddMistralTextCompletion(
                modelId: mistralConfiguration.ModelId,
                apiKey: "INVALID_KEY",
                serviceId: mistralConfiguration.ServiceId)
            .Build();

        IReadOnlyKernelPluginCollection plugins = TestHelpers.ImportSamplePlugins(target, "SummarizePlugin");

        // Act and Assert
        var ex = await Assert.ThrowsAsync<HttpOperationException>(() => target.InvokeAsync(plugins["SummarizePlugin"]["Summarize"], new() { [InputParameterName] = "Any" }));

        Assert.Equal(HttpStatusCode.Unauthorized, ((HttpOperationException)ex).StatusCode);
    }

    [Fact]
    public async Task MistralHttpExceededMaxTokensShouldReturnErrorDetailAsync()
    {
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();
        Assert.NotNull(mistralConfiguration);

        // Arrange
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._testOutputHelper);
        Kernel target = this._kernelBuilder
            .AddMistralTextCompletion(
                modelId: mistralConfiguration.ModelId,
                apiKey: mistralConfiguration.ApiKey,
                serviceId: mistralConfiguration.ServiceId)
            .Build();

        IReadOnlyKernelPluginCollection plugins = TestHelpers.ImportSamplePlugins(target, "SummarizePlugin");

        // Act
        // Assert
        await Assert.ThrowsAsync<HttpOperationException>(() => plugins["SummarizePlugin"]["Summarize"].InvokeAsync(target, new() { [InputParameterName] = string.Join('.', Enumerable.Range(1, 40000)) }));
    }

    [Fact]
    public async Task MistralInvokePromptTestAsync()
    {
        // Arrange
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        var builder = this._kernelBuilder;
        this.ConfigureChatMistral(builder);
        Kernel target = builder.Build();

        var prompt = "Where is the most famous fish market in Seattle, Washington, USA?";

        // Act
        FunctionResult actual = await target.InvokePromptAsync(prompt);

        // Assert
        Assert.Contains("Pike Place", actual.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task MistralDefaultValueTestAsync()
    {
        // Arrange
        this._kernelBuilder.Services.AddSingleton<ILoggerFactory>(this._logger);
        var builder = this._kernelBuilder;
        this.ConfigureChatMistral(builder);
        Kernel target = builder.Build();

        IReadOnlyKernelPluginCollection plugin = TestHelpers.ImportSamplePlugins(target, "FunPlugin");

        // Act
        FunctionResult actual = await target.InvokeAsync(plugin["FunPlugin"]["Limerick"]);

        // Assert
        Assert.Contains("Bob", actual.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
    }

    #region internals

    private readonly XunitLogger<Kernel> _logger;
    private readonly RedirectOutput _testOutputHelper;

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MistralCompletionTests()
    {
        this.Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._logger.Dispose();
            this._testOutputHelper.Dispose();
        }
    }

    private void ConfigureChatMistral(IKernelBuilder kernelBuilder)
    {
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();

        Assert.NotNull(mistralConfiguration);
        Assert.NotNull(mistralConfiguration.ModelId);
        Assert.NotNull(mistralConfiguration.ApiKey);
        Assert.NotNull(mistralConfiguration.ServiceId);

        kernelBuilder.AddMistralChatCompletion(
            modelId: mistralConfiguration.ModelId,
            apiKey: mistralConfiguration.ApiKey,
            serviceId: mistralConfiguration.ServiceId);
    }

    private void ConfigureInvalidMistral(IKernelBuilder kernelBuilder)
    {
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();
        Assert.NotNull(mistralConfiguration);
        Assert.NotNull(mistralConfiguration.ModelId);

        kernelBuilder.AddMistralTextCompletion(
            modelId: mistralConfiguration.ModelId,
            apiKey: "invalid-api-key",
            serviceId: $"invalid-{mistralConfiguration.ServiceId}");
    }

    private void ConfigureMistralChatAsText(IKernelBuilder kernelBuilder)
    {
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();

        Assert.NotNull(mistralConfiguration);
        Assert.NotNull(mistralConfiguration.ApiKey);
        Assert.NotNull(mistralConfiguration.ServiceId);

        kernelBuilder.AddMistralChatCompletion(
            modelId: mistralConfiguration.ModelId,
            apiKey: mistralConfiguration.ApiKey,
            serviceId: mistralConfiguration.ServiceId);
    }
    private void ConfigureMistralText(IKernelBuilder kernelBuilder)
    {
        var mistralConfiguration = this._configuration.GetSection("Mistral").Get<MistralConfiguration>();

        Assert.NotNull(mistralConfiguration);
        Assert.NotNull(mistralConfiguration.ApiKey);
        Assert.NotNull(mistralConfiguration.ServiceId);

        kernelBuilder.AddMistralTextCompletion(

            modelId: mistralConfiguration.ModelId,
            apiKey: mistralConfiguration.ApiKey,
            serviceId: mistralConfiguration.ServiceId);
    }
    #endregion
}
