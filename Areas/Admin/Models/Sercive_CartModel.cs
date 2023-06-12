
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{

    public class Service_CartModel
    {
        public int IdServie { get; set; }
        public string IdUser { get; set; }
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        [Column(TypeName = "date")]
        public DateTime? dateTime { get; set; }
    }
 
}
