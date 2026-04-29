using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalAnalyzedReviews { get; set; }
        public decimal AverageQuality { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageDelivery { get; set; }
        public decimal AverageService { get; set; }
        public decimal AverageOverall { get; set; }
    }
}
