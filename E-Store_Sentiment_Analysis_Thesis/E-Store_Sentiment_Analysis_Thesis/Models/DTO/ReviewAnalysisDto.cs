using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace E_Store_Sentiment_Analysis_Thesis.Models.DTO
{
    
    public class ReviewAnalysisDto
    {
        public string ReviewText { get; set; }
        public float? PriceScore { get; set; }
        public float? QualityScore { get; set; }
        public float? DeliveryScore { get; set; }
        public float? ServiceScore { get; set; }
        public float? OverallScore { get; set; }
    }
    }
