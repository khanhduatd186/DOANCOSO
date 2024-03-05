using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebBanThu.Areas.Admin.Models;
using WebBanThu.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    //[Authorize]
    public class AdminBillController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Bill");
            List<BillModel> Bills = JsonConvert.DeserializeObject<List<BillModel>>(datajson);
            List<Bill> bills = new List<Bill>();
            foreach(var i in Bills)
            {
                string data = await client.GetStringAsync("api/Account/GetUserById/" + i.IdUser);
                UserModel user = JsonConvert.DeserializeObject<UserModel>(data);
                var bill = new Bill { Id = i.Id, Price = i.Price, dateTime = i.dateTime, Name = user.Name , Status = i.Status };
                bills.Add(bill);
            }
            return View(bills);

        }
      
        [HttpGet]

        public async Task<IActionResult> Create()

        {

            return View();

        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create(BillModel p)

        {

            try
            {
                client.BaseAddress = new Uri(domain);
                string data = JsonConvert.SerializeObject(p);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("api/Bill", content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Bill Created";
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
        public async Task<IActionResult> Edit(int id)

        {
            try
            {
                client.BaseAddress = new Uri(domain);
                BillModel Bill = new BillModel();
                HttpResponseMessage response = client.GetAsync("api/Bill/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data1 = response.Content.ReadAsStringAsync().Result;

                    Bill = JsonConvert.DeserializeObject<BillModel>(data1);

                }
                BillModel billModel = new BillModel
                {
                    Id = Bill.Id,
                    IdUser = Bill.IdUser,
                    Price = Bill.Price,
                    dateTime = Bill.dateTime,
                    IsDelete = Bill.IsDelete,
                    Status = 1,

                };
                string data = JsonConvert.SerializeObject(billModel);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PutAsync("api/Bill/" + id, content).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Bill Created";
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


        [ActionName("HuyDon")]
        public async Task<IActionResult> HuyDon(int id)
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            BillModel bill = new BillModel();
            string data = await client.GetStringAsync("api/Bill/" + id);
            
            List<Product_BillModel> product_Bills = new List<Product_BillModel>();
            bill = JsonConvert.DeserializeObject<BillModel>(data);

            BillModel billModel = new BillModel
            {
                Id = bill.Id,
                dateTime = bill.dateTime,
                IdUser = bill.IdUser,
                IsDelete = 0,
                Price = bill.Price,
                Status = 3,
            };
            string data4 = JsonConvert.SerializeObject(billModel);
            StringContent content4 = new StringContent(data4, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage4 = client.PutAsync("api/Bill/" + id, content4).Result;
            if (responseMessage4.IsSuccessStatusCode)
            {
                string data5 = await client.GetStringAsync("api/Product_Bill/" + id);
                product_Bills = JsonConvert.DeserializeObject<List<Product_BillModel>>(data5);
                foreach( Product_BillModel product_Bill1 in product_Bills)
                {
                    try
                    {
                        string data6 = await client.GetStringAsync("api/Product/" + product_Bill1.IdProduct);
                        ProductModel product = JsonConvert.DeserializeObject<ProductModel>(data6);
                        ProductModel productModel = new ProductModel
                        {
                            Id = product.Id,
                            CategoryId = product.CategoryId,
                            Description = product.Description,
                            Image = product.Image,
                            Price = product.Price,
                            Quantity = product.Quantity,
                            Tittle = product.Tittle,
                            Isdelete = 0
                        };
                        string data7 = JsonConvert.SerializeObject(productModel);
                        StringContent content5 = new StringContent(data7, Encoding.UTF8, "application/json");
                        HttpResponseMessage responseMessage5 = client.PutAsync("api/Product/" + productModel.Id, content5).Result;
                    }
                    catch
                    {
                        return BadRequest();
                    }
                   

                }



                return RedirectToAction(nameof(Index));

            }
            return BadRequest();

        }
        [HttpGet]

        public async Task<IActionResult> Details(int id)

        {

            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);

                BillModel Bill = new BillModel();
                HttpResponseMessage response = client.GetAsync("api/Bill/" + id).Result;
   
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Bill = JsonConvert.DeserializeObject<BillModel>(data);

                    HttpResponseMessage datajson = client.GetAsync("api/Account/GetUserById/" + Bill.IdUser).Result;
                    string data1 = datajson.Content.ReadAsStringAsync().Result;
                    SignUpModel signUp = JsonConvert.DeserializeObject<SignUpModel>(data1);
                    ViewBag.Name = signUp.Name;
                }
                return View(Bill);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }



        }
        public async Task<IActionResult> Details_Product(int id)

        {

            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);

                BillModel Bill = new BillModel();
                HttpResponseMessage response = client.GetAsync("api/Bill/" + id).Result;
                List<Product_BillModel> product_Bills = new List<Product_BillModel>();
                List<Product_Bill> products = new List<Product_Bill>();
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Bill = JsonConvert.DeserializeObject<BillModel>(data);

                    HttpResponseMessage datajson = client.GetAsync("api/Product_Bill/" + Bill.Id).Result;
                    string data1 = datajson.Content.ReadAsStringAsync().Result;
                    product_Bills = JsonConvert.DeserializeObject<List<Product_BillModel>>(data1);
                    foreach (var i in product_Bills)
                    {
                        string datajson1 = await client.GetStringAsync("api/Product/" + i.IdProduct);
                        ProductModel productModel = JsonConvert.DeserializeObject<ProductModel>(datajson1);
                        var newproduct = new Product_Bill { Tittle = productModel.Tittle, Price = i.Price, Quantity = i.Quantity };

                        products.Add(newproduct);
                    }

                }
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        public async Task<IActionResult> Details_Service(int id)

        {

            try
            {
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);

                BillModel Bill = new BillModel();
                HttpResponseMessage response = client.GetAsync("api/Bill/" + id).Result;
                List<Service_BillModel> service_Bills = new List<Service_BillModel>();
                List<Service_Bill> service_Bill = new List<Service_Bill>();
                if (response.IsSuccessStatusCode)
                {
                   
                    string data = response.Content.ReadAsStringAsync().Result;
                    Bill = JsonConvert.DeserializeObject<BillModel>(data);
                    HttpResponseMessage datajson = client.GetAsync("api/Service_Bill/" + Bill.Id).Result;
                    string data1 = datajson.Content.ReadAsStringAsync().Result;
                    
                    service_Bills = JsonConvert.DeserializeObject<List<Service_BillModel>>(data1);
                    foreach (var i in service_Bills)
                    {
                        string datajson1 = await client.GetStringAsync("api/Service/" + i.IdService);
                        ServiceModel serviceModel = JsonConvert.DeserializeObject<ServiceModel>(datajson1);
                        var newService = new Service_Bill { Tittle = serviceModel.Tittle, Price = i.Price, Quantity = i.Quantity };

                        service_Bill.Add(newService);
                    }

                }
                return View(service_Bill);
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
                BillModel Bill = new BillModel();
                HttpResponseMessage response = client.GetAsync("api/Bill/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    Bill = JsonConvert.DeserializeObject<BillModel>(data);
                }
                return View(Bill);
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

                HttpResponseMessage response = client.DeleteAsync("api/Bill/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Bill delete";
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
