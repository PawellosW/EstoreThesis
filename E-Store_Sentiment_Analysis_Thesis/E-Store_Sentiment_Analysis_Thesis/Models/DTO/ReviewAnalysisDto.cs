using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace E_Store_Sentiment_Analysis_Thesis.Models.DTO
{
    
    public class ReviewAnalysisDto
    {
        public string ReviewText { get; set; }
        public decimal? PriceScore { get; set; }
        public decimal? QualityScore { get; set; }
        public decimal? DeliveryScore { get; set; }
        public decimal? ServiceScore { get; set; }
        public decimal? OverallScore { get; set; }
    }
    }
