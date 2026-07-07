using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDashboardAnalysisService _dashboardService;

        public DashboardController(IServiceScopeFactory serviceScopeFactory, IDashboardAnalysisService dashboardService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var model = _dashboardService.GetDashboardSummary();


            return View(model);
        }

        

        [HttpPost]
        public IActionResult TriggerAnalysis()
        {
            // 1. Ścieżka do skryptu - najlepiej trzymać ją w folderze projektu
            string pythonScript = Path.Combine(Directory.GetCurrentDirectory(), "AnalysisModule", "master_worker.py");

            // 2. Uruchomienie procesu w tle
            Task.Run(async () =>
            {
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        FileName = "python", // Upewnij się, że python jest w zmiennych środowiskowych (PATH)
                        Arguments = $"\"{pythonScript}\"",
                        WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "AnalysisModule"),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (Process process = new Process { StartInfo = start })
                    {
                        process.OutputDataReceived += (sender, e) =>  // ← usuń async
                        {
                            if (!string.IsNullOrEmpty(e.Data) && e.Data.StartsWith("###ID:"))
                            {
                                try
                                {
                                    var parts = e.Data.Split("###");
                                    int id = int.Parse(parts[1].Replace("ID:", ""));
                                    string output = parts[2].Replace("OUT:", "");

                                    using (var scope = _serviceScopeFactory.CreateScope())
                                    {
                                        var service = scope.ServiceProvider.GetRequiredService<IDashboardAnalysisService>();
                                        // .GetAwaiter().GetResult() zamiast await — bezpieczne w event handlerze
                                        service.SaveSingleAnalysisResultToDb(id, output).GetAwaiter().GetResult();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("[PARSE ERR] " + ex.Message); // ← teraz błędy będą widoczne
                                }
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                Console.WriteLine("[PYTHON ERR] " + e.Data);
                        };

                        process.Start();
                        process.BeginOutputReadLine(); // Rozpocznij nasłuchiwanie
                        process.BeginErrorReadLine();
                        process.WaitForExit();
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
