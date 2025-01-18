using AiChatTest.Models;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AiChatTest;

public static class ChatClientFactory
{
    public static IChatClient CreateChatClient(ChatType chatType)
    {
        switch (chatType)
        {
            case ChatType.Ollama:
                return new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.1");
            case ChatType.OllamaWithTools:
                return new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.1")
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();
            case ChatType.OpenAi:
                return new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                    .AsChatClient(modelId: "gpt-4o-mini");
            case ChatType.AzureAIInference:
            case ChatType.AzureOpenAi:
            default:
                throw new ArgumentOutOfRangeException(nameof(chatType), chatType, null);
        }
    }
}