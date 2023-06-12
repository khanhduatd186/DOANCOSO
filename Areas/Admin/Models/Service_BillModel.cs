using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
  
    public class Service_BillModel
    {
        public int IdService { get; set; }
        public int IdBill { get; set; }
        [Range(0, 100)]
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }

    }
}
