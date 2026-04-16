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

        List<ReviewAnalysisDto> ISentimentParserService.ParseMultipleAiOutputs(string rawOutput)
        {
            var results = new List<ReviewAnalysisDto>();

            if (string.IsNullOrWhiteSpace(rawOutput))
                return results;

            // KROK 1: Czyszczenie "manier" modelu LLM. 
            // Zamieniamy każde "ScorE" (niezależnie od wielkości liter) na "Score".
            // To załatwi problem z "PricEscorE", "QualityScorE" itd.
            string cleanedOutput = Regex.Replace(rawOutput, "ScorE", "Score", RegexOptions.IgnoreCase);

            // KROK 2: Wyciąganie poszczególnych obiektów JSON.
            // Szukamy tekstu między klamrami { }, ale "leniwie" (*?), 
            // żeby nie połączyć wszystkich opinii w jeden wielki blok.
            var matches = Regex.Matches(cleanedOutput, @"\{.*?\}", RegexOptions.Singleline);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignoruje wielkość liter przy dopasowywaniu do DTO
            };

            // KROK 3: Deserializacja każdego znalezionego kawałka
            foreach (Match match in matches)
            {
                try
                {
                    var dto = JsonSerializer.Deserialize<ReviewAnalysisDto>(match.Value, options);

                    if (dto != null)
                    {
                        results.Add(dto);
                    }
                }
                catch (JsonException)
                {
                    // Jeśli jeden JSON jest ucięty lub błędny, logujemy błąd 
                    // i przechodzimy do następnego, żeby nie wywalać całej analizy.
                    continue;
                }
            }

            return results;
        }
    }
}
