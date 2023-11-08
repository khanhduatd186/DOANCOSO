using ApiPetShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class AdminAccountController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        [HttpGet]

        public async Task<IActionResult> Index()

        {

            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            List<UserModel> Users = new List<UserModel>();
            HttpResponseMessage responseMessage = client.GetAsync("api/Account").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                Users = JsonConvert.DeserializeObject<List<UserModel>>(data);
            }

            return View(Users);

        }
        [HttpGet]

        public async Task<IActionResult> Create()

        {

            return View();

        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel p)
        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Account", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User Created";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> AddRoleAdmin()
        {
           
            return View();
        }
        [HttpPost, ActionName("AddRoleAdmin")]
        public async Task<IActionResult> AddRoleAdmin(UpdatePermissionModel updatePermission)
        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(updatePermission);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Account/make-admin", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Add Role Done";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
    }
}
