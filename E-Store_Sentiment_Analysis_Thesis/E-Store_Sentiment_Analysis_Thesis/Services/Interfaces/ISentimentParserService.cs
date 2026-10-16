using E_Store_Sentiment_Analysis_Thesis.Models.DTO;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface ISentimentParserService
    {
        ReviewAnalysisDto ParseSingleAiOutput(string rawOutput);
    }
}
