using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using E_Store_Sentiment_Analysis_Thesis.ViewModels;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Implementations
{
    public class DashboardAnalysisService : IDashboardAnalysisService
    {
        private readonly StoreContext _context;
        private readonly ISentimentParserService _parser;

        public DashboardAnalysisService(StoreContext context, ISentimentParserService parser)
        {
            _context = context;
            _parser = parser;
        }
       

        public async Task SaveAnalysisResultToDb(string outputFilePath)
        {

            if (!System.IO.File.Exists(outputFilePath))
            {
                throw new FileNotFoundException("Nie znaleziono pliku pod ścieżką: " + outputFilePath);
            }

            string rawAiOutput = await System.IO.File.ReadAllTextAsync(outputFilePath);

            var results = _parser.ParseMultipleAiOutputs(rawAiOutput);

            // pobiera same ID opinii wykorzystanych już w analizie
            var usedIds = _context.ReviewAnalyses.Select(a => a.ReviewId).ToList();
            // pobiera Id opinii, które nie zostały wykorzystane w usedIds
            var freeIds = _context.Reviews
                .Where(r => !usedIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToList();

            for (int i = 0; i < results.Count; i++)
            {
                if (i >= freeIds.Count) break;

                var dto = results[i];
                _context.ReviewAnalyses.Add(new ReviewAnalysis
                {
                    ReviewId = freeIds[i],
                    QualityScore = dto.QualityScore,
                    PriceScore = dto.PriceScore,
                    DeliveryScore = dto.DeliveryScore,
                    ServiceScore = dto.ServiceScore,
                    OverallScore = dto.OverallScore
                    
                });
            }
            await _context.SaveChangesAsync();
        }


        public DashboardViewModel GetDashboardSummary()
        {
            // Pobieramy wszystkie analizy z bazy
            var allAnalyses = _context.ReviewAnalyses.ToList();
            var model = new DashboardViewModel();

            model.TotalAnalyzedReviews = allAnalyses.Count;

            if (allAnalyses.Any())
            {
                // Liczymy średnie (odrzucamy wartości null, jeśli jakieś były)
                model.AverageQuality = allAnalyses.Average(a => a.QualityScore) ?? 0;
                model.AveragePrice = allAnalyses.Average(a => a.PriceScore) ?? 0;
                model.AverageDelivery = allAnalyses.Average(a => a.DeliveryScore) ?? 0;
                model.AverageService = allAnalyses.Average(a => a.ServiceScore) ?? 0;
                model.AverageOverall = allAnalyses.Average(a => a.OverallScore) ?? 0;

                // Zaokrąglamy do 2 miejsc po przecinku
                model.AverageQuality = Math.Round(model.AverageQuality, 2);
                model.AveragePrice = Math.Round(model.AveragePrice, 2);
                model.AverageDelivery = Math.Round(model.AverageDelivery, 2);
                model.AverageService = Math.Round(model.AverageService, 2);
                model.AverageOverall = Math.Round(model.AverageOverall, 2);
            }

        

            return model;
        }

        public List<ReviewAnalysis> GetAllAnalyses()
        {
            return _context.ReviewAnalyses.ToList();
        }
    }
}
