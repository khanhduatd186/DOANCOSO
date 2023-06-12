using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Data;

namespace WebBanThu.Areas.Admin.Controllers
{
    
    [Authorize(Roles ="ADMIN")]
    [Area("Admin")]
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View("Vào Admin không thành công");
        }
    }
}
