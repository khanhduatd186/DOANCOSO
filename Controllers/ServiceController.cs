using ApiPetShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using WebBanThu.Areas.Admin.Models;
using WebBanThu.Models;

namespace WebBanThu.Controllers
{
    public class ServiceController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        public async Task<IActionResult> Index()
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Service/GET_ALL");
            List<ServiceModel> services = JsonConvert.DeserializeObject<List<ServiceModel>>(datajson);

           

            ViewBag.Services = services;
           

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            client.BaseAddress = new Uri(domain);
       
       
            try
            {
                ViewBag.Domain = domain;
                List<Service_DetailModel> list = new List<Service_DetailModel>();
                HttpResponseMessage response = client.GetAsync("api/Service_Detail/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data1 = response.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Service_DetailModel>>(data1);
                    ServiceModel service = new ServiceModel();
                    HttpResponseMessage responseMessage = client.GetAsync("api/Service/"+id).Result;
                    string data = responseMessage.Content.ReadAsStringAsync().Result;
                    service = JsonConvert.DeserializeObject<ServiceModel>(data);
                    ViewBag.ServiceTittle = service.Tittle;
                    ViewBag.Service = service;
                    ViewBag.List = list;
                    List<TimeModel> times = new List<TimeModel>();
                    foreach(var i in list)
                    {
                        string datajson1 = await client.GetStringAsync("api/Time/" + i.IdTime);
                        TimeModel time = JsonConvert.DeserializeObject<TimeModel>(datajson1);
        
                        times.Add(time);
    
                    }
                    ViewBag.Times = times;
                    ViewBag.Times1 = new SelectList(times, "Id", "StartTime");

                }
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            


            //string datajson1 = await client.GetStringAsync("api/Time");
            //List<TimeModel> times = JsonConvert.DeserializeObject<List<TimeModel>>(datajson1);
            //

            return View();

            
        }
        //[HttpGet]
        //public async Task<IActionResult> DatLich()
        //{
        //    return View();
        //}
        [HttpPost]
        [Authorize(Roles = "CUTOMER")]
        public async Task<IActionResult> DatLich(string email, int IdService, string Time, string SelectedTime)
        {
            client.BaseAddress = new Uri(domain);
            HttpResponseMessage datajson = client.GetAsync("api/Account/GetUseByEmail/" + email).Result;
            string data1 = datajson.Content.ReadAsStringAsync().Result;
            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(data1);
            Service_CartModel p = new Service_CartModel
            {
                Time = SelectedTime,
                IdServie = IdService,
                IdUser = user.id,
                Quantity = 0,
                Price = 0,
                dateTime = DateTime.Now
            };
            try
            {

              
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Service_Cart", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service_Cart Created";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

            return View(p);
        }
        [Authorize(Roles = "CUTOMER")]
        public async Task<IActionResult> LichDaDat()
        {

            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            HttpResponseMessage datajson = client.GetAsync("api/Account/GetUseByEmail/" + User.Identity.Name).Result;
            string data1 = datajson.Content.ReadAsStringAsync().Result;
            SignUpModel user = JsonConvert.DeserializeObject<SignUpModel>(data1);
            List<Service_CartModel> Service_Details = new List<Service_CartModel>();
            List<Service_Cart> Service = new List<Service_Cart>();
            HttpResponseMessage responseMessage = client.GetAsync("api/Service_Cart/"+user.id).Result;
            if (responseMessage.IsSuccessStatusCode)
            {

                string data = responseMessage.Content.ReadAsStringAsync().Result;
                Service_Details = JsonConvert.DeserializeObject<List<Service_CartModel>>(data);
                foreach (var i in Service_Details)
                {
                    HttpResponseMessage datajson1 = client.GetAsync("api/Service/" + i.IdServie).Result;
                    string data2 = datajson1.Content.ReadAsStringAsync().Result;
                    ServiceModel service = JsonConvert.DeserializeObject<ServiceModel>(data2);
                    Service_Cart service_Cart = new Service_Cart { dateTime = i.dateTime, Name = user.Name, Title = service.Tittle, Time = i.Time };
                    Service.Add(service_Cart);
                }
            }
                

            return View(Service);
        }




    }
}
