using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class Service_Bill
    {
        public string Tittle { get; set; }
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
    }
}
