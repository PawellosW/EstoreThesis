using Microsoft.AspNetCore.Builder;
using E_Store_Sentiment_Analysis_Thesis.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using E_Store_Sentiment_Analysis_Thesis.Models;
using System.Globalization;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using E_Store_Sentiment_Analysis_Thesis.Services.Implementations;

namespace E_Store_Sentiment_Analysis_Thesis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStoreReviewService, StoreReviewService>();
            services.AddScoped<ISentimentParserService, SentimentParserService>();
            services.AddScoped<IDashboardAnalysisService, DashboardAnalysisService>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/Login";
                });

            services.AddSession();
            services.AddControllersWithViews();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(CultureInfo.InvariantCulture);
                options.SupportedCultures = new[] { CultureInfo.InvariantCulture };
                options.SupportedUICultures = new[] { CultureInfo.InvariantCulture };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseRequestLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Products}/{action=Index}/{id?}");
            });
        }
    }
}