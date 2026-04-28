using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Models;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface IDashboardAnalysisService
    {
        Task SaveAnalysisResultToDb(string outputFilePath);

        // Zwracamy surową listę z bazy danych
        List<ReviewAnalysis> GetAllAnalyses();
    }
}
