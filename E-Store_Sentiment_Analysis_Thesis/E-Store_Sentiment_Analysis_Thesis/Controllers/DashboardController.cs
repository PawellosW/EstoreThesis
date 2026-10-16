using E_Store_Sentiment_Analysis_Thesis.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace E_Store_Sentiment_Analysis_Thesis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDashboardAnalysisService _dashboardService;

        private static readonly bool DemoMode = true;

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
        [ValidateAntiForgeryToken]
        public IActionResult TriggerAnalysis()
        {

            if (DemoMode)
            {
                TempData["Success"] = "Uruchamianie analizy jest niedostępne w wersji demonstracyjnej.";
                return RedirectToAction("Index");
            }

            
            string pythonScript = Path.Combine(Directory.GetCurrentDirectory(), "AnalysisModule", "master_worker.py");

            
            Task.Run(async () =>
            {
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{pythonScript}\"",
                        WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "AnalysisModule"),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (Process process = new Process { StartInfo = start })
                    {
                        process.OutputDataReceived += (sender, e) => 
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
                                        
                                        service.SaveSingleAnalysisResultToDb(id, output).GetAwaiter().GetResult();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("[PARSE ERR] " + ex.Message);
                                }
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                Console.WriteLine("[PYTHON ERR] " + e.Data);
                        };

                        process.Start();
                        process.BeginOutputReadLine(); 
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine("Błąd startu skryptu: " + ex.Message);
                }
            });

            TempData["Success"] = "Analiza została zlecona. Odśwież stronę za chwilę, aby zobaczyć pierwsze wyniki.";
            return RedirectToAction("Index");
        }
    }
}
