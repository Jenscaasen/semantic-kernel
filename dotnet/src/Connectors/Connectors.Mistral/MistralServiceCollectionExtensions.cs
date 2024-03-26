// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Mistral;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.TextGeneration;

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable IDE0039 // Use local function

namespace Microsoft.SemanticKernel;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> and related classes to configure Mistral and Azure Mistral connectors.
/// </summary>
public static class MistralServiceCollectionExtensions
{
    #region Chat Completion

    /// <summary>
    /// Adds the Mistral chat completion service to the list.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="apiKey">Azure Mistral API key, see https://learn.microsoft.com/azure/cognitive-services/Mistral/quickstart</param>
    /// <param name="modelId">Model identifier</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddMistralChatCompletion(
        this IKernelBuilder builder,
        string apiKey,
        string modelId,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        Verify.NotNull(builder);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
        {
            return new(modelId,
               apiKey, HttpClientProvider.GetHttpClient(httpClient, serviceProvider));
        };

        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);

        return builder;
    }

    /// <summary>
    /// Adds the Mistral chat completion service to the list.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="modelId">Mistral model name</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddMistralChatCompletion(
        this IServiceCollection services,
        string modelId,
        string apiKey,
        string? serviceId = null)
    {
        Verify.NotNull(services);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
        {
            return new(modelId,
               apiKey, HttpClientProvider.GetHttpClient(serviceProvider));
        };

        services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);

        return services;
    }
    /// <summary>
    /// Adds the Mistral text completion service to the list.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="modelId">Model identifier</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddMistralTextCompletion(
        this IKernelBuilder builder,
        string apiKey,
        string modelId,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        Verify.NotNull(builder);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
        {
            return new(modelId,
               apiKey, HttpClientProvider.GetHttpClient(httpClient, serviceProvider));
        };

        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, factory);

        return builder;
    }

    /// <summary>
    /// Adds the Mistral text completion service to the list.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="modelId">Mistral model name</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddMistralTextCompletion(
        this IServiceCollection services,
        string modelId,
        string apiKey,
        string? serviceId = null)
    {
        Verify.NotNull(services);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
        {
            return new(modelId,
               apiKey, HttpClientProvider.GetHttpClient(serviceProvider));
        };

        services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);

        return services;
    }

    #region Azure Mistral
    /// <summary>
    /// Adds the Azure-hosted Mistral chat completion service to the list.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="endpoint">Mistral endpoint</param>
    /// <param name="modelId">Mistral model name</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddAzureMistralChatCompletion(
        this IServiceCollection services,
        string endpoint,
        string modelId,
        string apiKey,
        string? serviceId = null)
    {
        Verify.NotNull(services);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
        {
            return new(modelId,
               apiKey, HttpClientProvider.GetHttpClient(serviceProvider), endpoint);
        };

        services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);

        return services;
    }
    /// <summary>
    /// Adds the Azure-hosted Mistral text completion service to the list.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="endpoint">Mistral endpoint</param>
    /// <param name="modelId">Mistral model name</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddAzureMistralTextCompletion(
        this IServiceCollection services,
        string endpoint,
        string modelId,
        string apiKey,
        string? serviceId = null)
    {
        Verify.NotNull(services);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
        {
            return new(modelId,
               apiKey, HttpClientProvider.GetHttpClient(serviceProvider), endpoint);
        };

        services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);

        return services;
    }
    #endregion
    #endregion

    #region Text Embedding

    /// <summary>
    /// Adds the Mistral text embeddings service to the list.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="modelId">Mistral model name</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddMistralTextEmbeddingGeneration(
        this IKernelBuilder builder,
        string modelId,
        string apiKey,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        Verify.NotNull(builder);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Services.AddKeyedSingleton<ITextEmbeddingGenerationService>(serviceId, (serviceProvider, _) =>
            new MistralTextEmbeddingGenerationService(
                modelId,
                apiKey,
               HttpClientProvider.GetHttpClient(httpClient, serviceProvider),
                serviceProvider.GetService<ILoggerFactory>()));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        return builder;
    }

    /// <summary>
    /// Adds the Mistral text embeddings service to the list.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to augment.</param>
    /// <param name="modelId">Mistral model name</param>
    /// <param name="apiKey">Mistral API key</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="httpClient">The HttpClient to use</param>
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddMistralTextEmbeddingGeneration(
        this IServiceCollection services,
        string modelId,
        string apiKey,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        Verify.NotNull(services);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return services.AddKeyedSingleton<ITextEmbeddingGenerationService>(serviceId, (serviceProvider, _) =>
            new MistralTextEmbeddingGenerationService(
                modelId,
                apiKey,
                 HttpClientProvider.GetHttpClient(httpClient, serviceProvider),
                serviceProvider.GetService<ILoggerFactory>()));
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }

    #endregion

}
