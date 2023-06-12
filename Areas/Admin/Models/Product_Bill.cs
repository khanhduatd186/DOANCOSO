using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class Product_Bill
    {
        public int id { get; set; }
        public string Tittle { get; set; }
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
    }
}
