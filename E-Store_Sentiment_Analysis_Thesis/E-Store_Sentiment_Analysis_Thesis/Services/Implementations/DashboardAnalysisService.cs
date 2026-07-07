using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Data;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using E_Store_Sentiment_Analysis_Thesis.ViewModels;
using Microsoft.EntityFrameworkCore;

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


        public async Task SaveSingleAnalysisResultToDb(int reviewId, string rawAiOutput)
        {
            
            var dto = _parser.ParseSingleAiOutput(rawAiOutput);

            if (dto == null) return;

            var existingAnalysis = await _context.ReviewAnalyses
            .FirstOrDefaultAsync(ra => ra.ReviewId == reviewId);

            if (existingAnalysis == null)
            {
                var analysis = new ReviewAnalysis
                {
                    ReviewId = reviewId,
                    QualityScore = dto.QualityScore,
                    PriceScore = dto.PriceScore,
                    DeliveryScore = dto.DeliveryScore,
                    ServiceScore = dto.ServiceScore,
                    OverallScore = dto.OverallScore,
                    IsUrgent = dto.IsUrgent
                   
                };

                _context.ReviewAnalyses.Add(analysis);
            }
            else
            {
                existingAnalysis.IsUrgent = dto.IsUrgent;
                _context.ReviewAnalyses.Update(existingAnalysis);
            }
            await _context.SaveChangesAsync();
        }


        public DashboardViewModel GetDashboardSummary()
        {
            // Pobieramy wszystkie analizy z bazy
            var allAnalyses = _context.ReviewAnalyses.ToList();
            var model = new DashboardViewModel();

            model.TotalAnalyzedReviews = allAnalyses.Count;

            model.TotalAspectsAnalyzed = allAnalyses
                .Count(a => a.OverallScore != null);

            model.TotalAlertsAnalyzed = allAnalyses
                .Count(a => a.IsUrgent != null);

            model.UrgentReviewsCount = allAnalyses
                .Count(a => a.IsUrgent == true);

            var today = DateTime.Today;

           

            // Poziom 1 — brak jakiegokolwiek rekordu analizy
            model.PendingLevel1Count = _context.Reviews
                .Count(r => !_context.ReviewAnalyses
                    .Any(ra => ra.ReviewId == r.Id));

            // Poziom 2 — jest rekord ale IsUrgent jest null
            model.PendingLevel2Count = _context.Reviews
                .Count(r => _context.ReviewAnalyses
                    .Any(ra => ra.ReviewId == r.Id && ra.IsUrgent == null));


            // Alerty szczegółowe
            model.UrgentReviews = _context.ReviewAnalyses
                .Where(ra => ra.IsUrgent == true)
                .Join(_context.Reviews,
                    ra => ra.ReviewId,
                    r => r.Id,
                    (ra, r) => new UrgentReviewItem
                    {
                        ReviewId = r.Id,
                        ReviewText = r.Text
                    })
                .ToList();

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
