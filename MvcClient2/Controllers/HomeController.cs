using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient2.Models;

namespace MvcClient2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            ViewData["Message"] = "Secure page.";

            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            //var content = await client.GetStringAsync("http://localhost:5001/identity?scopeId=aaa");

            var response = await client.PostAsJsonAsync("http://localhost:5001/identity", 
                                                        new {ScopeId = "3d0a1642-c2e1-4031-96a9-4fc651c245c1", Name = "test"})
                                       .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Json = response.ReasonPhrase;
                return View("json");
            }
            var content = await response.Content.ReadAsStringAsync();

            ViewBag.Json = content;
            return View("json");
        }
    }
}
