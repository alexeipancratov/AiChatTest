using AiChatTest;
using AiChatTest.Models;
using Microsoft.Extensions.AI;

var client = ChatClientFactory.CreateChatClient(ChatType.OllamaWithTools);

var chatHistory = new List<ChatMessage>();
var chatOptions = new ChatOptions
{
    Tools = ToolsProvider.GetBookTools()    
};

while (true)
{
    Console.Write(">>> ");
    var prompt = Console.ReadLine();

    if (prompt != null && prompt.Equals("bye", StringComparison.InvariantCultureIgnoreCase))
    {
        break;
    }
    
    chatHistory.Add(new ChatMessage(ChatRole.User, prompt));
    
    // ===== Atomic response =====
    var response = await client.CompleteAsync(chatHistory, chatOptions);
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));
    Console.WriteLine(response.Message.Text ?? "<no response>");

    // ===== Real-time response =====
    // var response = string.Empty;
    //
    // await foreach (var token in client.CompleteStreamingAsync(chatHistory))
    // {
    //     Console.Write(token.Text);
    //     response += token.Text;
    // }
    // chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    //
    // Console.WriteLine();
}