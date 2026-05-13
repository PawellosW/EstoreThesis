using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Models.DTO;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Implementations
{
    public class SentimentParserService : ISentimentParserService
    {

        
            private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignoruje wielkość liter przy dopasowywaniu do DTO
            };

            /// <summary>
            /// NOWA METODA: Parsuje pojedynczy wynik wysłany przez Master Workera.
            /// </summary>
            public ReviewAnalysisDto ParseSingleAiOutput(string rawOutput)
            {
                if (string.IsNullOrWhiteSpace(rawOutput))
                    return null;

                try
                {
                    // KROK 1: Czyszczenie "manier" modelu (np. ScorE -> Score)
                    string cleaned = CleanRawString(rawOutput);

                    // KROK 2: Wyciągnięcie JSONa (na wypadek, gdyby model coś dopisał przed/po klamrach)
                    var match = Regex.Match(cleaned, @"\{.*?\}", RegexOptions.Singleline);

                    if (!match.Success)
                        return null;

                    // KROK 3: Deserializacja
                    return JsonSerializer.Deserialize<ReviewAnalysisDto>(match.Value, _jsonOptions);
                }
                catch (JsonException)
                {
                    // Logowanie błędu (opcjonalnie)
                    return null;
                }
            }

            

            // Pomocnicza metoda, żeby nie powtarzać Regexa do czyszczenia tekstu
            private string CleanRawString(string input)
            {
                return Regex.Replace(input, "ScorE", "Score", RegexOptions.IgnoreCase);
            }
    }
}
