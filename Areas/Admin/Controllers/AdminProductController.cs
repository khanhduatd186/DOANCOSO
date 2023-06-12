using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    public class AdminProductController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Product/GET_ALL");
            
            List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(datajson);
            List<Product> newlist = new List<Product>();
            foreach(var i in products)
            {
                string datajson1 = await client.GetStringAsync("api/Category/" +i.CategoryId);
                CategoryModel categoryModels = JsonConvert.DeserializeObject<CategoryModel>(datajson1);
                var newProduct = new Product { Id = i.Id, Description = i.Description, Image = i.Image, Tittle = i.Tittle, Price = i.Price, Isdelete = i.Isdelete, Quantity = i.Quantity, NameCategory = categoryModels.Tittle };
                newlist.Add(newProduct);
            }
            return View(newlist);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Category/GET_ALL");
            List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(datajson);
            ViewBag.CategoryId = new SelectList(categories, "Id", "Tittle");
            return View();

        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel p, IFormFile file)

        {

            try
            {
                client.BaseAddress = new Uri(domain);
                var formdata = new MultipartFormDataContent();
                //formdata.Add(new StringContent(p.Id.ToString()), "Id");
                formdata.Add(new StringContent(p.Tittle), "Tittle");
                formdata.Add(new StringContent(p.Description), "Description");
                formdata.Add(new StreamContent(file.OpenReadStream()), "Picture", file.FileName);
                formdata.Add(new StringContent(p.Quantity.ToString()), "Quantity");
                formdata.Add(new StringContent(p.Price.ToString()), "Price");
                formdata.Add(new StringContent(p.CategoryId.ToString()), "CategoryId");
                formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");

                //string data = JsonConvert.SerializeObject(formdata);
                //StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("api/Product/Uploadfile", formdata).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "product created";
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

        public async Task<IActionResult> Edit(int id, ProductModel p, IFormFile file)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                var formdata = new MultipartFormDataContent();
                formdata.Add(new StringContent(p.Id.ToString()), "Id");
                formdata.Add(new StringContent(p.Tittle), "Tittle");
                formdata.Add(new StringContent(p.Description), "Description");
                formdata.Add(new StreamContent(file.OpenReadStream()), "Picture", file.FileName);
                formdata.Add(new StringContent(p.Quantity.ToString()), "Quantity");
                formdata.Add(new StringContent(p.Price.ToString()), "Price");
                formdata.Add(new StringContent(p.CategoryId.ToString()), "CategoryId");
                formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");

                //string data = JsonConvert.SerializeObject(formdata);
                //StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync("api/Product/updateloadFile/" + id, formdata).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "product edit";
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
                ProductModel product = new ProductModel();
                string datajson = await client.GetStringAsync("api/Category/GET_ALL");

                List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(datajson);

                ViewBag.CategoryId = new SelectList(categories, "Id", "Tittle");
                HttpResponseMessage response = client.GetAsync("api/Product/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    product = JsonConvert.DeserializeObject<ProductModel>(data);

                }
                return View(product);
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
                ProductModel product = new ProductModel();
 
                HttpResponseMessage response = client.GetAsync($"api/Product/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    product = JsonConvert.DeserializeObject<ProductModel>(data);
                    HttpResponseMessage datajson = client.GetAsync("api/Category/"+product.CategoryId).Result;
                    string data1 = datajson.Content.ReadAsStringAsync().Result;
                    CategoryModel categorie = JsonConvert.DeserializeObject<CategoryModel>(data1);
                    ViewBag.Tittle = categorie.Tittle;

                }
                return View(product);
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
                ProductModel product = new ProductModel();
                HttpResponseMessage response = client.GetAsync("api/Product/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    product = JsonConvert.DeserializeObject<ProductModel>(data);

                }
                return View(product);
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

                HttpResponseMessage response = client.DeleteAsync("api/Product/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "product delete";
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
