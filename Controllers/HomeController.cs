using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebBanThu.Areas.Admin.Models;
using WebBanThu.Models;

namespace WebBanThu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
  
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            string datajson = await client.GetStringAsync("api/Service/GET_ALL");
            List<ServiceModel> services = JsonConvert.DeserializeObject<List<ServiceModel>>(datajson).Take(3).ToList();

            string datajson1 = await client.GetStringAsync("api/Product/GET_ALL");
            List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(datajson1).Take(6).ToList();
            ViewBag.Products = products;

            ViewBag.Services = services;


            return View(); 
        }
        [Authorize(Roles = "CUTOMER")]
        public async Task<IActionResult> DanhSachBillByKH()
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            string email = User.Identity.Name;
            SignUpModel user = new SignUpModel();
            string data = await client.GetStringAsync("api/Account/GetUseByEmail/" + email);
            user = JsonConvert.DeserializeObject<SignUpModel>(data);
            string datajson = await client.GetStringAsync("api/Bill/GetListBill/" + user.id);
            List<BillModel> Bills = JsonConvert.DeserializeObject<List<BillModel>>(datajson);
            List<Bill> bills = new List<Bill>();
            foreach (var i in Bills)
            {
                string data1 = await client.GetStringAsync("api/Account/GetUserById/" + i.IdUser);
                UserModel user1 = JsonConvert.DeserializeObject<UserModel>(data);
                var bill = new Bill { Id = i.Id, Price = i.Price, dateTime = i.dateTime, Name = user.Name,Status = i.Status };
                bills.Add(bill);
            }
            return View(bills);

        }
        [Authorize(Roles = "CUTOMER")]
        public async Task<IActionResult> XemChiTietHoaDon(int IdBill)
        {
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);

            string datajson = await client.GetStringAsync("api/Product_Bill/" + IdBill);
            List<Product_BillModel> Bills = JsonConvert.DeserializeObject<List<Product_BillModel>>(datajson);
            List<Product_Bill> bills = new List<Product_Bill>();
            foreach (var i in Bills)
            {
                string data1 = await client.GetStringAsync("api/Product/" + i.IdProduct);
                ProductModel product = JsonConvert.DeserializeObject<ProductModel>(data1);
                var product_bill = new Product_Bill { id = i.IdBill, Tittle = product.Tittle, Quantity = i.Quantity, Price = i.Price };
                bills.Add(product_bill);
            }
            return View(bills);

        }

      

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}