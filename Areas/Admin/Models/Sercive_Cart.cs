
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{

    public class Service_Cart
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public DateTime? dateTime { get; set; }
    }
 
}
