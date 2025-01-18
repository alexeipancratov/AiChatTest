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
        async Task<IReadOnlyCollection<Book>> GetBooks()
        {
            LogToConsole("GetBooks");
            
            await using var connection = new SqliteConnection("Data Source=/Users/alexeipancratov/Documents/databases/books.sqlite");
            await connection.OpenAsync();
            var query = "SELECT Id, Name, CAST(Rating AS REAL) AS Rating FROM Books";
            var books = await connection.QueryAsync<Book>(query);

            return books.ToArray();
        }

        return
        [
            AIFunctionFactory.Create(GetBooks,
                new AIFunctionFactoryCreateOptions
                {
                    Name = "get_books",
                    Description = "Retrieves books",
                })
        ];
    }
}
