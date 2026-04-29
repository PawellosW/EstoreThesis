using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.ViewModels;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface IDashboardAnalysisService
    {
        Task SaveAnalysisResultToDb(string outputFilePath);

        // DashboardViewModel w tym przypadku oznacza typ zwracany. GetDashboardSummary to nazwa operacji należącej do serwisu.
        DashboardViewModel GetDashboardSummary();
    }
}
