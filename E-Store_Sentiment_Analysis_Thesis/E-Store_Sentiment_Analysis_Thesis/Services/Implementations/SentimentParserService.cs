using E_Store_Sentiment_Analysis_Thesis.Models.DTO;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Implementations
{
    public class SentimentParserService : ISentimentParserService
    {

        
            private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

       
            public ReviewAnalysisDto ParseSingleAiOutput(string rawOutput)
            {
                if (string.IsNullOrWhiteSpace(rawOutput))
                    return null;

                try
                {
                    
                    string cleaned = CleanRawString(rawOutput);

                    var match = Regex.Match(cleaned, @"\{.*?\}", RegexOptions.Singleline);

                    if (!match.Success)
                        return null;

                   
                    return JsonSerializer.Deserialize<ReviewAnalysisDto>(match.Value, _jsonOptions);
                }
                catch (JsonException)
                {
                    
                    return null;
                }
            }

            

            private string CleanRawString(string input)
            {
                return Regex.Replace(input, "ScorE", "Score", RegexOptions.IgnoreCase);
            }
    }
}
