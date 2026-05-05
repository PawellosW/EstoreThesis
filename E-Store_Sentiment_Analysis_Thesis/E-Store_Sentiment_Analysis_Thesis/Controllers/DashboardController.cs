using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using System.IO;
using System.Diagnostics;

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

        [HttpPost]
        public IActionResult TriggerAnalysis()
        {
            // 1. Ścieżka do skryptu - najlepiej trzymać ją w folderze projektu
            string pythonScript = Path.Combine(Directory.GetCurrentDirectory(), "AnalysisModule", "aspects_rate_worker.py");

            // 2. Uruchomienie procesu w tle
            Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        FileName = "python", // Upewnij się, że python jest w zmiennych środowiskowych (PATH)
                        Arguments = pythonScript,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (Process process = Process.Start(start))
                    {
                        // Tutaj proces sobie leci w tle systemu operacyjnego
                    }
                }
                catch (Exception ex)
                {
                    // Logowanie błędów, jeśli skrypt się nawet nie odpalił
                    Console.WriteLine("Błąd startu skryptu: " + ex.Message);
                }
            });

            TempData["Success"] = "Analiza została zlecona. Odśwież stronę za chwilę, aby zobaczyć pierwsze wyniki.";
            return RedirectToAction("Index");
        }
    }
}
