using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    public class AdminCategoryController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        [HttpGet]

        public async Task<IActionResult> Index()

        {

            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            List<CategoryModel> Categorys = new List<CategoryModel>();
            HttpResponseMessage responseMessage = client.GetAsync("api/Category/GET_ALL").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                Categorys =  JsonConvert.DeserializeObject<List<CategoryModel>>(data);

            }
  
            return View(Categorys);

        }

        [HttpGet]

        public async Task<IActionResult> Create()

        {

            return View();

        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create(CategoryModel p)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Category", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Category Created";
                    return RedirectToAction("Index");
                }
            }catch(Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
            
        }


        [HttpPost]

        public async Task<IActionResult> Edit(int id, CategoryModel p)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PutAsync("api/Category/"+id, content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Category Created";
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


        public async Task<IActionResult> Edit(int id)

        {
            client.BaseAddress = new Uri(domain);
            CategoryModel Category = new CategoryModel();
            HttpResponseMessage response = client.GetAsync("api/Category/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Category = JsonConvert.DeserializeObject<CategoryModel>(data);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            return View(Category);
        }
        [HttpGet]

        public async Task<IActionResult> Details(int id)

        {
            client.BaseAddress = new Uri(domain);
            CategoryModel Category = new CategoryModel();
            HttpResponseMessage response = client.GetAsync("api/Category/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Category = JsonConvert.DeserializeObject<CategoryModel>(data);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            return View(Category);

        }

        [HttpGet]

        public async Task<IActionResult> Delete(int Id)

        {
            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                CategoryModel Category = new CategoryModel();
                HttpResponseMessage response = client.GetAsync("api/Category/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    Category = JsonConvert.DeserializeObject<CategoryModel>(data);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
                return View(Category);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)

        {
            try
            {
                client.BaseAddress = new Uri(domain);

                HttpResponseMessage response = client.DeleteAsync("api/Category/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Category delete";
                    return RedirectToAction("Index");
                }
                return View();
            }

            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
    }
}
