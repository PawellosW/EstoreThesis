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
        [Column("id")]
        public int Id { get; set; }

        [Column("review_id")]
        public int ReviewId { get; set; }

        [Column("price_score")]
        public decimal? PriceScore { get; set; }

        [Column("quality_score")]
        public decimal? QualityScore { get; set; }

        [Column("delivery_score")]
        public decimal? DeliveryScore { get; set; }

        [Column("service_score")]
        public decimal? ServiceScore { get; set; }

        [Column("overall_score")]
        public decimal? OverallScore { get; set; }

        
        [Column("is_urgent")]
        public bool? IsUrgent { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }


    }
}
