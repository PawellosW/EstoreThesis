using System.Collections.Generic;

namespace E_Store_Sentiment_Analysis_Thesis.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalAnalyzedReviews { get; set; }
        public int UrgentReviewsCount { get; set; }
		public int PendingLevel1Count { get; set; }
       
		
        public decimal AverageQuality { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageDelivery { get; set; }
        public decimal AverageService { get; set; }
    
        
        public List<UrgentReviewItem> UrgentReviews { get; set; }

    }
}
