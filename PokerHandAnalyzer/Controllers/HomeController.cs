using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PokerHandAnalyzer.Models;

namespace PokerHandAnalyzer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PokerHandModel()); // Pass an empty model on initial load
    }

    [HttpPost]
    public IActionResult Index(PokerHandModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Return view with validation errors
        }

        if (model.HasDuplicateCards())
        {
            ModelState.AddModelError("", "Duplicate cards are not allowed.");
            return View(model);
        }

        // Process the hand (replace with actual poker logic)
        model.ResultMessage = "Hand analysis completed successfully!";

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
