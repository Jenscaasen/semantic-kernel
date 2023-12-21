// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Mistral;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.SemanticKernel.TextToImage;

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable IDE0039 // Use local function

namespace Microsoft.SemanticKernel;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> and related classes to configure OpenAI and Azure OpenAI connectors.
/// </summary>
public static class MistralServiceCollectionExtensions
{
    #region Chat Completion

    /// <summary>
    /// Adds the Mistral chat completion service to the list.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder"/> instance to augment.</param>
    /// <param name="apiKey">Azure OpenAI API key, see https://learn.microsoft.com/azure/cognitive-services/openai/quickstart</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="modelId">Model identifier</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder"/>.</returns>
    public static IKernelBuilder AddMistralChatCompletion(
        this IKernelBuilder builder,
        string apiKey, string modelId,
        string? serviceId = null,       
        HttpClient? httpClient = null)
    {

        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
           new(modelId,
               apiKey);

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
    /// <returns>The same instance as <paramref name="services"/>.</returns>
    public static IServiceCollection AddMistralChatCompletion(
        this IServiceCollection services,
        string modelId,
        string apiKey,
        string? serviceId = null)
    {
     
        Func<IServiceProvider, object?, MistralAIChatCompletionService> factory = (serviceProvider, _) =>
            new(modelId,
                apiKey);

        services.AddKeyedSingleton<IChatCompletionService>(serviceId, factory);

        return services;
    }

    #endregion

}
