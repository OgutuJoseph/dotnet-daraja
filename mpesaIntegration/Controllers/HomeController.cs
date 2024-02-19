using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mpesaIntegration.Models;

namespace mpesaIntegration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<string> GetToken() 
    {
        var client = _clientFactory.CreateClient("mpesa");
        var authString = "ITNqYY0yJE0qF1hVbug5PmD0y6MdFWBG:IzCv1CjMkrrYI3Py";
        var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));
        var url = "/oauth/v1/generate?grant_type=client_credentials";
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
