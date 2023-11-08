using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    //[Authorize]
    public class AdminServiceController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        public async Task<IActionResult> Index()
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Service/GET_ALL");

            List<ServiceModel> Services = JsonConvert.DeserializeObject<List<ServiceModel>>(datajson);

            return View(Services);

        }

        [HttpGet]

        public async Task<IActionResult> Create()

        {
           
            return View();

        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create(ServiceModel p, IFormFile file)

        {

            try
            {
                client.BaseAddress = new Uri(domain);
                var formdata = new MultipartFormDataContent();
                //formdata.Add(new StringContent(p.Id.ToString()), "Id");
                formdata.Add(new StringContent(p.Tittle), "Tittle");

                formdata.Add(new StringContent(p.Description), "Description");

                formdata.Add(new StreamContent(file.OpenReadStream()), "Picture", file.FileName);
                formdata.Add(new StringContent(p.Price.ToString()), "Price");
                formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");
         
                HttpResponseMessage response = client.PostAsync("api/Service/uploadfile", formdata).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service created";
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


        [HttpPost]

        public async Task<IActionResult> Edit(int id, ServiceModel p, IFormFile file)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(p.Id.ToString()), "Id");
                formdata.Add(new StringContent(p.Tittle), "Tittle");

                formdata.Add(new StringContent(p.Description), "Description");

                formdata.Add(new StreamContent(file.OpenReadStream()), "Picture", file.FileName);
                formdata.Add(new StringContent(p.Price.ToString()), "Price");
                formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");
          

                //string data = JsonConvert.SerializeObject(formdata);
                //StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync("api/Service/updateloadFile/" + id, formdata).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service edit";
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
        [HttpGet]


        public async Task<IActionResult> Edit(int id)

        {


            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                ServiceModel Service = new ServiceModel();
   
                HttpResponseMessage response = client.GetAsync("api/Service/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    Service = JsonConvert.DeserializeObject<ServiceModel>(data);

                }
                return View(Service);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpGet]

        public async Task<IActionResult> Details(int id)

        {

            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
 
                ServiceModel Service = new ServiceModel();
                HttpResponseMessage response = client.GetAsync("api/Service/"+id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Service = JsonConvert.DeserializeObject<ServiceModel>(data);
                }
                return View(Service);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        } 

        public async Task<IActionResult> IndexDetail(int id)

        {
            
            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                List<Service_DetailModel> list = new List<Service_DetailModel>();
                HttpResponseMessage response = client.GetAsync("api/Service_Detail/" + id).Result;
            
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Service_DetailModel>>(data);
                }
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }

        [HttpGet]

        public async Task<IActionResult> Delete(int Id)

        {
            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                ServiceModel Service = new ServiceModel();
                HttpResponseMessage response = client.GetAsync("api/Service/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Service = JsonConvert.DeserializeObject<ServiceModel>(data);

                }
                return View(Service);
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

                HttpResponseMessage response = client.DeleteAsync("api/Service/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Service delete";
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
