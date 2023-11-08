using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Data;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Areas.Admin.Controllers
{
    
    [Authorize(Roles ="ADMIN")]
    [Area("Admin")]
    public class AdminHomeController : Controller
    {
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();
        public async Task<IActionResult> Index()
        {
            try
            {
                int SoluongBill = 0;
                double TongDoanhThu = 0;
                int SoTaiKhoan = 0;
                int TongSanPhamBan = 0;
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                string datajson = await client.GetStringAsync("api/Bill");
                List<BillModel> Bills = JsonConvert.DeserializeObject<List<BillModel>>(datajson);
                string datajson1 = await client.GetStringAsync("api/Account");
                List<UserModel> user = JsonConvert.DeserializeObject<List<UserModel>>(datajson1);
                string datajson2 = await client.GetStringAsync("api/Product_Bill");
                List<Product_BillModel> product_Bills = JsonConvert.DeserializeObject<List<Product_BillModel>>(datajson2);

                SoluongBill = Bills.Count();
                SoTaiKhoan = user.Count();
                TongDoanhThu = Bills.Sum(s => s.Price);
                TongSanPhamBan = product_Bills.Sum(p => p.Quantity);
                ThongKe thongKe = new ThongKe
                {
                    SoLuongBil = SoluongBill,
                    SoLuongTaiKhoan = SoTaiKhoan,
                    TongDoanhThu = TongDoanhThu,
                    TongSanPhamDaBan = TongSanPhamBan,
                    
                };
                return View(thongKe);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View("Vào Admin không thành công");
        }
    }
}
