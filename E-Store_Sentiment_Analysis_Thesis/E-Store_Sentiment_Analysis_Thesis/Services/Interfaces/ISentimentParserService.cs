using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Models;
using E_Store_Sentiment_Analysis_Thesis.Models.DTO;

namespace E_Store_Sentiment_Analysis_Thesis.Services.Interfaces
{
    public interface ISentimentParserService
    {
        ReviewAnalysisDto ParseSingleAiOutput(string rawOutput);
    }
}
