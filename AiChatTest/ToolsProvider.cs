using AiChatTest.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI;

namespace AiChatTest;

public static class ToolsProvider
{
    private static void LogToConsole(string message)
    {
        var originalForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine($"Function call: {message}");
        
        Console.ForegroundColor = originalForegroundColor;
    }
    
    public static IList<AITool> GetBookTools()
    {
        return
        [
            AIFunctionFactory.Create(GetBooks,
                new AIFunctionFactoryCreateOptions
                {
                    Name = "get_books",
                    Description = "Retrieves books",
                })
        ];

        async Task<IReadOnlyCollection<Book>> GetBooks()
        {
            LogToConsole("GetBooks");
            
            await using var connection = new SqliteConnection("Data Source=Database/books.sqlite");
            await connection.OpenAsync();
            var query = "SELECT * FROM Books WHERE UserId = @UserId";
            var books = await connection.QueryAsync<Book>(query, new { UserId = 100 });

            return books.ToArray();
        }
    }
}
