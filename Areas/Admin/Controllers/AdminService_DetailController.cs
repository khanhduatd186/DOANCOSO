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
    public class AdminService_DetailController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        [HttpGet]

        public async Task<IActionResult> Index()
        {

            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            List<Service_DetailModel> Service_Details = new List<Service_DetailModel>();
            List<Service_Detail> services = new List<Service_Detail>();
            HttpResponseMessage responseMessage = client.GetAsync("api/Service_Detail").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                string data = responseMessage.Content.ReadAsStringAsync().Result;
                Service_Details = JsonConvert.DeserializeObject<List<Service_DetailModel>>(data);
                foreach(var i in Service_Details)
                {
                    string datajson1 = await client.GetStringAsync("api/Time/"+i.IdTime);
                    TimeModel time = JsonConvert.DeserializeObject<TimeModel>(datajson1);

                    var newService_Detail = new Service_Detail {IdTime = i.IdTime, IdService = i.IdService, NgayThucHien = i.NgayThucHien, SoLuongCa = i.SoLuongCa, ThoiGian = time.StartTime };
                    services.Add(newService_Detail);
                }
            }

            return View(services);

        }

        [HttpGet]

        public async Task<IActionResult> Create()

        {
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Time");
            List<TimeModel> times = JsonConvert.DeserializeObject<List<TimeModel>>(datajson);
            ViewBag.Time = new SelectList(times, "Id", "StartTime");
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create(Service_DetailModel p)

        {
            try
            {
                
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Service_Detail", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service_Detail Created";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            TempData["errorC"] = "Đã tồn tại Lịch";
            ViewBag.Loi = TempData["errorC"];
            return View(p);

        }


        [HttpPost]

        public async Task<IActionResult> Edit(int idService,int idTime, Service_DetailModel p)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PutAsync($"api/Services/{idService}/Times/{idTime}", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service_Detail Created";
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


        public async Task<IActionResult> Edit(int idService, int idTime )

        {
            client.BaseAddress = new Uri(domain);
            Service_DetailModel Service_Detail = new Service_DetailModel();
            HttpResponseMessage response = client.GetAsync($"api/services/{idService}/Times/{idTime}").Result;
            string datajson = await client.GetStringAsync("api/Time");
            List<TimeModel> times = JsonConvert.DeserializeObject<List<TimeModel>>(datajson);
            ViewBag.Time = new SelectList(times, "Id", "StartTime");
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                Service_Detail = JsonConvert.DeserializeObject<Service_DetailModel>(data);
            }
            return View(Service_Detail);
        }
        [HttpGet]

        public async Task<IActionResult> Details(int idService, int idTime )

        {
            client.BaseAddress = new Uri(domain);
         
            Service_DetailModel Service_Detail = new Service_DetailModel();
            HttpResponseMessage response = client.GetAsync($"api/services/{idService}/Times/{idTime}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                Service_Detail = JsonConvert.DeserializeObject<Service_DetailModel>(data);
                HttpResponseMessage datajson = client.GetAsync("api/Time/" + Service_Detail.IdTime).Result;
                string data1 = datajson.Content.ReadAsStringAsync().Result;
                TimeModel time = JsonConvert.DeserializeObject<TimeModel>(data1);
                ViewBag.StartTime = time.StartTime;
            }
            return View(Service_Detail);

        }

        [HttpGet]

        public async Task<IActionResult> Delete(int IdService, int IdTime)

        {
            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                Service_DetailModel Service_Detail = new Service_DetailModel();
                HttpResponseMessage response = client.GetAsync($"api/services/{IdService}/Times/{IdTime}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    Service_Detail = JsonConvert.DeserializeObject<Service_DetailModel>(data);
                    HttpResponseMessage datajson = client.GetAsync("api/Time/" + Service_Detail.IdTime).Result;
                    string data1 = datajson.Content.ReadAsStringAsync().Result;
                    TimeModel time = JsonConvert.DeserializeObject<TimeModel>(data1);
                    ViewBag.StartTime = time.StartTime;

                }
                return View(Service_Detail);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed([FromForm]int IdService, [FromForm] int IdTime)

        {
            try
            {
                client.BaseAddress = new Uri(domain);

                HttpResponseMessage response = client.DeleteAsync($"api/Services/{IdService}/Times/{IdTime}").Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service_Detail delete";
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
