// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Mistral;
using Microsoft.SemanticKernel.TextGeneration;
using Xunit;

namespace SemanticKernel.Connectors.UnitTests.Mistral.ChatCompletion;

/// <summary>
/// Unit tests for <see cref="MistralChatCompletionService"/>
/// </summary>
public sealed class MistralChatCompletionTests : IDisposable
{
    private readonly HttpMessageHandlerStub _messageHandlerStub;
    private readonly HttpClient _httpClient;

    public MistralChatCompletionTests()
    {
        this._messageHandlerStub = new HttpMessageHandlerStub();
        this._httpClient = new HttpClient(this._messageHandlerStub, false);

    }

    [Fact]
    public async Task ItGetChatMessageContentsShouldHaveModelIdDefinedAsync()
    {
        // Arrange
        var chatCompletion = new MistralAITextAndChatCompletionService(modelId: "mistral-tiny", apiKey: "NOKEY", httpClient: this._httpClient);
        this._messageHandlerStub.ResponseToReturn = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        { Content = new StringContent(MistralChatCompletionResponse, Encoding.UTF8, "application/json") };

        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.User, "What is the best French cheese?");

        // Act
        var chatMessage = await chatCompletion.GetChatMessageContentAsync(chatHistory);

        // Assert
        Assert.NotNull(chatMessage.ModelId);
        Assert.Equal("mistral-tiny", chatMessage.ModelId);
    }

    [Fact]
    public async Task ItGetTextContentsShouldHaveModelIdDefinedAsync()
    {
        // Arrange
        var chatCompletion = new MistralAITextAndChatCompletionService(modelId: "mistral-tiny", apiKey: "NOKEY", httpClient: this._httpClient);
        this._messageHandlerStub.ResponseToReturn = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        { Content = new StringContent(MistralChatCompletionResponse, Encoding.UTF8, "application/json") };

        // Act
        var textContent = await chatCompletion.GetTextContentAsync("What is the best French cheese?");

        // Assert
        Assert.NotNull(textContent.ModelId);
        Assert.Equal("mistral-tiny", textContent.ModelId);
    }

    public void Dispose()
    {
        this._httpClient.Dispose();
        this._messageHandlerStub.Dispose();
    }

    private const string MistralChatCompletionResponse = @"{
    ""id"": ""cmpl-ee80cdbac8b547e2a379e076a27b92db"",
    ""object"": ""chat.completion"",
    ""created"": 1703160012,
    ""model"": ""mistral-tiny"",
    ""choices"": [
        {
            ""index"": 0,
            ""message"": {
                ""role"": ""assistant"",
                ""content"": ""Determining the \""best\"" French cheese is subjective and depends on personal preferences, as there are over 400 types of French cheeses, each with unique flavors, textures, and milk sources. Some popular and highly regarded French cheeses include:\n\n1. Roquefort: A blue-veined sheep's milk cheese from the Massif Central region, known for its strong, pungent aroma and tangy, savory flavor.\n\n2. Comté: A firm, nutty, and slightly sweet cow's milk cheese from the Franche-Comté region, often aged for several years for added complexity.\n\n3. Camembert: A soft, creamy cow's milk cheese from Normandy, famous for its earthy, mushroomy flavor and white, downy rind.\n\n4. Brie de Meaux: A soft, bloomy-rind cow's milk cheese from the Île-de-France region, characterized by its rich, buttery taste and velvety texture.\n\n5. Munster: A pungent, smelly, and runny cow's milk cheese from the Alsace region, with a strong ammonia aroma and a complex, cheesy flavor.\n\n6. Chaource: A soft, bloomy-rind cow's milk cheese from the Île-de-France region, known for its mild, buttery flavor and creamy texture.\n\n7. Époisses: A pungent, smelly cow's milk cheese from Burgundy, with a runny, gooey texture and a strong, complex flavor that is both savory and sweet.\n\n8. Morbier: A semi-soft, cow's milk cheese from Franche-Comté, with a distinctive ash layer running through the middle, adding a subtle smoky flavor to the cheese.\n\nThese are just a few examples of the many delicious French cheeses available. To find your favorite, you might consider trying a variety of French cheeses and exploring different milk sources, textures, and flavors.""
            },
            ""finish_reason"": ""stop""
        }
    ],
    ""usage"": {
        ""prompt_tokens"": 15,
        ""total_tokens"": 475,
        ""completion_tokens"": 460
    }
}";
}
