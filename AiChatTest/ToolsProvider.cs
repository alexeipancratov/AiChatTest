using System.Text.RegularExpressions;
using AiChatTest.Models;
using Dapper;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI;

namespace AiChatTest;

public class ToolsProvider(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    private void LogToConsole(string message)
    {
        var originalForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine($"Function call: {message}");
        
        Console.ForegroundColor = originalForegroundColor;
    }
    
    public IList<AITool> GetBookTools()
    {
        return
        [
            AIFunctionFactory.Create(GetBooks,
                new AIFunctionFactoryCreateOptions
                {
                    Name = "get_books",
                    Description = "Retrieves books",
                }),
            AIFunctionFactory.Create(GetBookRating,
                new AIFunctionFactoryCreateOptions
                {
                    Name = "get_book_rating",
                    Description = "Retrieves book rating",
                    Parameters =
                    [
                        new AIFunctionParameterMetadata("bookName")
                        {
                            Description = "The book name"
                        }
                    ]
                }),
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

        async Task<double> GetBookRating(string bookName)
        {
            LogToConsole("GetBookRating");

            try
            {
                var response = await _httpClient.GetAsync($"https://goodreads.com/search?utf8=✓&q={bookName}");
                response.EnsureSuccessStatusCode();
                var html = await response.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
            
                var bookNode = doc.DocumentNode.SelectSingleNode("//tr[@itemscope and @itemtype='http://schema.org/Book']");

                var ratingNode = bookNode?.SelectSingleNode(".//span[@class='minirating']");
            
                if (ratingNode == null) return 0.0;
                var ratingText = ratingNode.InnerText.Trim();
                var ratingMatch = Regex.Match(ratingText, @"(\d+\.\d+)?");
            
                return ratingMatch.Success ? double.Parse(ratingMatch.Value) : 0.0;
            }
            catch (Exception e)
            {
                LogToConsole(e.Message);
                return 0.0;
            }
        }
    }
}
