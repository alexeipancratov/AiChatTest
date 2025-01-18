using AiChatTest;
using AiChatTest.Models;
using Microsoft.Extensions.AI;

var client = ChatClientFactory.CreateChatClient(ChatType.OpenAi);

var chatMessages = new List<ChatMessage>();

while (true)
{
    Console.Write(">>> ");
    var prompt = Console.ReadLine();

    if (prompt != null && prompt.Equals("bye", StringComparison.InvariantCultureIgnoreCase))
    {
        break;
    }
    
    chatMessages.Add(new ChatMessage(ChatRole.User, prompt));
    
    var response = string.Empty;

    await foreach (var token in client.CompleteStreamingAsync(chatMessages))
    {
        Console.Write(token.Text);
        response += token.Text;
    }
    chatMessages.Add(new ChatMessage(ChatRole.Assistant, response));
    
    Console.WriteLine();
}