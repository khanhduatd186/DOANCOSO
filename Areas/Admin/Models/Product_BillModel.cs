using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanThu.Areas.Admin.Models
{ 
    public class Product_BillModel
    {
        public int IdProduct { get; set; }
        public int IdBill { get; set; }
        [Range(0, 100)]
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }


    }
}
