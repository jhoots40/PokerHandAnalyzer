using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using PokerEquityCalculator;
using PokerHandAnalyzer.Models;
using PokerHandAnalyzer.Services;

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
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { validationErrors = errors });
        }

        if (model.HasDuplicateCards())
        {
            return Json(new { validationErrors = new List<string> { "Duplicate cards are not allowed." } });
        }

        // Initialize PokerService
        var pokerService = new PokerService();

        // Call equity calculation
        List<Double> equities = pokerService.GetEquity(model.HeroHand, model.VillainHand, model.CommunityCards);

        // Process the hand (replace with actual poker logic)
        string resultMessage = $"Hero Equity: {equities[0]} Villian Equity: {equities[1]}";

        return Json(new { resultMessage = resultMessage });
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
