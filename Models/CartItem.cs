
using System.ComponentModel.DataAnnotations;
using WebBanThu.Areas.Admin.Models;

namespace WebBanThu.Models
{
    public class CartItem
    {
        public int quantity { set; get; }
        public ProductModel product { set; get; }

    }

}
