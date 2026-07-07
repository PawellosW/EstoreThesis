using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_Store_Sentiment_Analysis_Thesis.Models;
using Microsoft.AspNetCore.Identity;

namespace E_Store_Sentiment_Analysis_Thesis.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductsOrder> ProductsOrders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewAnalysis> ReviewAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Order>().ToTable("orders");
            modelBuilder.Entity<ProductsOrder>().ToTable("products_orders");
            modelBuilder.Entity<Review>().ToTable("reviews");
            modelBuilder.Entity<ReviewAnalysis>().ToTable("review_analysis");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<ProductsOrder>()
                .HasOne(po => po.Order)
                .WithMany(o => o.ProductsOrders)
                .HasForeignKey(po => po.OrderId);

            modelBuilder.Entity<ProductsOrder>()
                .HasOne(po => po.Product)
                .WithMany()
                .HasForeignKey(po => po.ProductId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .IsRequired(false);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .IsRequired(false);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ReviewAnalysis)   
                .WithOne(ra => ra.Review)       
                .HasForeignKey<ReviewAnalysis>(ra => ra.ReviewId);


        }
    }
}