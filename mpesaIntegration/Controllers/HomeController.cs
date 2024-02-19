using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using mpesaIntegration.Models;
using Newtonsoft.Json;

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
        var authString = "OF3t9e104IE8eYLlS15ur1YO4rpw5aP3:gDPw7eTaoVZyGlxs";
        var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));
        var _url = "/oauth/v1/generate?grant_type=client_credentials";

        var request = new HttpRequestMessage(HttpMethod.Get, _url);
        request.Headers.Add("Authorization", $"Basic {encodedString}");

        var response = await client.SendAsync(request);
        var mpesaResponse = await response.Content.ReadAsStringAsync();
        // Console.WriteLine("mpesa response");
        // Console.WriteLine(mpesaResponse);

        Token tokenObject = JsonConvert.DeserializeObject<Token>(mpesaResponse);

        return tokenObject.access_token;
    }

    class Token {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }

    // Register URL
    public IActionResult RegisterURLs() 
    {
        return View();
    }

    [HttpGet]
    [Route("register-urls")]
    public async Task<string> RegisterMpesURLs () 
    {
        var jsonBody = JsonConvert.SerializeObject(new {
            ValidationURL = "https://mydemo-url.com/confirmation",
            ConfirmationURL = "https://mydemo-url.com/validation",
            ResponseType = "Completed",
            ShortCode = 600988
        });

        var jsonReadyBody = new StringContent(
            jsonBody.ToString(),
            Encoding.UTF8,
            "application/json"
        );

        var token = await GetToken(); 
        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var url = "/mpesa/c2b/v1/registerurl";
        var response = await client.PostAsync(url, jsonReadyBody);

        return await response.Content.ReadAsStringAsync();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
