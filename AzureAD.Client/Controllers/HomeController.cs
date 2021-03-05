using AzureAD.Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureAD.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ITokenAcquisition tokenAcquisition;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.tokenAcquisition = tokenAcquisition;
        }

        [AuthorizeForScopes(Scopes = new[] { "apione", "api2" })]
        public async Task<IActionResult> Index()
        {
            //by default we will get an access token as we have added permissions for microsoft graph
            // check the audience value, if the audience claim contains a string with mostly 0's.
            // this access token is meant to access microsoftgraph.
            // to get an access token to access API1 we need to add it to the configuration
            //var accessToken = await HttpContext.GetTokenAsync("access_token");
            var scopes = new List<string> { "api1" };
            //  var scopes = new List<string> { "api://ddd95954-fecb-4fec-86de-999872ad9aba/readall" };
            var accessToken = await this.tokenAcquisition.GetAuthenticationResultForUserAsync(scopes);

            var httpClient = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                 HttpMethod.Get,
                 "https://localhost:44327/weatherforecast");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

            var response = await httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // error happened
            }

            scopes = new List<string> { "api2" };
            accessToken = await this.tokenAcquisition.GetAuthenticationResultForUserAsync(scopes);


            var request1 = new HttpRequestMessage(
                 HttpMethod.Get,
                 "https://localhost:44321/weatherforecast");
            request1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

            var response1 = await httpClient.SendAsync(request1);

            if (response1.StatusCode != HttpStatusCode.OK)
            {
                // error happened
            }
            return View();
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
}
