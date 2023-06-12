using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class Service_DetailModel
    {
        public int IdService { get; set; }
        public int IdTime { get; set; }
        public int SoLuongCa { get; set; }
   
        public DateTime NgayThucHien { get; set; }
    }
}
