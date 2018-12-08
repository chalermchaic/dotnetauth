using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace dotnetauth.Controllers
{
    [Route("/")]
    [ApiController]
    public class AppController : Controller
    {
        string client_id = "client_id";
        string client_secret = "client_secret";
        string redirect_uri = "https://localhost:5001/callback"; 
        string authorize_uri = "https://www.fitbit.com/oauth2/authorize";
        string token_uri = "https://api.fitbit.com/oauth2/token";

        public IActionResult Index(){
            return View();
        }
        //1: Authorize
        [HttpGet("login")]
        public ActionResult<string> Get()        
        {
            string authorize = authorize_uri+"?response_type=code&client_id="+client_id
            +"&redirect_uri="+redirect_uri 
            +"&scope=activity%20heartrate%20location%20nutrition%20profile%20settings%20sleep%20social%20weight&expires_in=604800";
            Console.WriteLine(authorize);
            return authorize;
        }

        //1A Get Code -> Token        
        [HttpGet("callback")]
        public ActionResult<object> callbackAsync(string code)        
        {
            var dict = new Dictionary<string, string>();
            dict.Add("clientId", client_id);
            dict.Add("grant_type", "authorization_code");
            dict.Add("redirect_uri", redirect_uri);
            dict.Add("code", code);
            
            var requestUri = new Uri(token_uri);
            var request = new HttpRequestMessage {
                RequestUri = requestUri,
                Method = HttpMethod.Post,
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), "Basic " + getBasicAuthorization() },
                    { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                },
                Content = new FormUrlEncodedContent(dict)                 
            };
            var client = new HttpClient();
            var response = client.SendAsync(request).Result;
            var token = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(token);

            return token; 
        }

        string getBasicAuthorization(){
            var text = client_id+":"+client_secret;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            var basic = System.Convert.ToBase64String(plainTextBytes);
            return basic;
        }
    }
}
