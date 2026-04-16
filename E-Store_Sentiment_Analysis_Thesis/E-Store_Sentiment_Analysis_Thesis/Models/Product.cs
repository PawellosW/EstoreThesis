using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace E_Store_Sentiment_Analysis_Thesis.Models
{
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get;  set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [Column("price", TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("stock")]
        public int? Stock { get; set; }
    }
}