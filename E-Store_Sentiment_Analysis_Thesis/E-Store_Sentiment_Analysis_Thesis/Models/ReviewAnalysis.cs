using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Store_Sentiment_Analysis_Thesis.Models
{
    public class ReviewAnalysis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }


        public decimal? PriceScore { get; set; }
        public decimal? QualityScore { get; set; }
        public decimal? DeliveryScore { get; set; }
        public decimal? ServiceScore { get; set; }
        public decimal? OverallScore { get; set; }
    }
}
