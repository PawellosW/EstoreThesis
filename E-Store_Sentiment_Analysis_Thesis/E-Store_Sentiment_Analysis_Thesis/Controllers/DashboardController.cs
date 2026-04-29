using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardAnalysisService _dashboardService;

        public DashboardController(IDashboardAnalysisService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var model = _dashboardService.GetDashboardSummary();


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImportManualResults()
        {
            
            string filePath = @"C:\Users\lenovo\Desktop\wyniki_aspekty.txt";

            try
            {
                // Kontroler tylko mówi CO zrobić, nie wie JAK czytany jest plik
                await _dashboardService.SaveAnalysisResultToDb(filePath);
                TempData["Success"] = "Dane zaimportowane pomyślnie!";
            }
            catch (Exception ex)
            {
                // Tutaj możesz obsłużyć błędy (np. brak pliku)
                TempData["Error"] = "Błąd: " + ex.Message;
            }


            return RedirectToAction("Index");
        }
    }
}
