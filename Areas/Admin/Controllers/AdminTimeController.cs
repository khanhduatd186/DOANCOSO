using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class AdminTimeController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        [HttpGet]

        public async Task<IActionResult> Index()

        {

            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            List<TimeModel> Times = new List<TimeModel>();
            HttpResponseMessage responseMessage = client.GetAsync("api/Time").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                Times = JsonConvert.DeserializeObject<List<TimeModel>>(data);
            }

            return View(Times);

        }

        [HttpGet]

        public async Task<IActionResult> Create()

        {

            return View();

        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(TimeModel p)
        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Time", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Time Created";
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


        [HttpPost]

        public async Task<IActionResult> Edit(int id, TimeModel p)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PutAsync("api/Time/" + id, content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Time Created";
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
            TimeModel Time = new TimeModel();
            HttpResponseMessage response = client.GetAsync("api/Time/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
              Time = JsonConvert.DeserializeObject<TimeModel>(data);

            }
            return View(Time);
        }
        [HttpGet]

        public async Task<IActionResult> Details(int id)

        {
            client.BaseAddress = new Uri(domain);
            TimeModel Time = new TimeModel();
            HttpResponseMessage response = client.GetAsync("api/Time/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;

                Time = JsonConvert.DeserializeObject<TimeModel>(data);

            }
            return View(Time);

        }

        [HttpGet]

        public async Task<IActionResult> Delete(int Id)

        {
            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                TimeModel Time = new TimeModel();
                HttpResponseMessage response = client.GetAsync("api/Time/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    Time = JsonConvert.DeserializeObject<TimeModel>(data);

                }
                return View(Time);
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

                HttpResponseMessage response = client.DeleteAsync("api/Time/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Time delete";
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
