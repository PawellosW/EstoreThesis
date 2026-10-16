using E_Store_Sentiment_Analysis_Thesis.ViewModels;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface IDashboardAnalysisService
    {
        Task SaveSingleAnalysisResultToDb(int reviewId, string rawAiOutput);

        DashboardViewModel GetDashboardSummary();
    }
}
