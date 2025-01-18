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
    
    var response = await client.CompleteAsync(chatMessages);
    chatMessages.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));

    Console.WriteLine(response.Message);
}