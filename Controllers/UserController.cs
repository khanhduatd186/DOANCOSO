
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NuGet.Configuration;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebBanThu.Models;

namespace WebBanThu.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();
        public UserController(
            IConfiguration configuration)
        {

            _configuration = configuration;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Loi()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> login()
        {
           
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> login(SignInModel signIn, string ReturnUrl)
        {

            client.BaseAddress = new Uri(domain);
            string data = JsonConvert.SerializeObject(signIn);
            AuthModel auth = new AuthModel();
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = client.PostAsync("api/Account/login", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                string datajson = responseMessage.Content.ReadAsStringAsync().Result;
                auth = JsonConvert.DeserializeObject<AuthModel>(datajson);
                if (auth.MessageT != null)
                {
                    var userPricipal = this.ValidateToken(auth.MessageT);
                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(10),
                        IsPersistent = false
                    };
                     await HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       userPricipal,
                       authProperties);
                    if (string.IsNullOrEmpty(ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(ReturnUrl);
                       
                        
                    }
                    
                }
                else
                {
                    TempData["errorMessage"] = "Invalid password";
                    return View(signIn);
                    
                }
               
            }
            else
            {
                TempData["errorMessage"] = "user not fond";
                return RedirectToAction("login", "User");

            }

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(SignUpModel signUpModel)
        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(signUpModel);
                AuthModel auth = new AuthModel();
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Account/register", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "accout Created";
                    return RedirectToAction("login", "User");
                }
            }
           
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("login", "User");
        }
      
         //giai ma token
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["JWT:ValidAudience"];
            validationParameters.ValidIssuer = _configuration["JWT:ValidIssuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}
