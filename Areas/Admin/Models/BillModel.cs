
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class BillModel
    {
       
        public int Id { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public DateTimeOffset dateTime { get; set; }

        public int Status { get; set; }
        public string? IdUser { get; set; }
        public byte IsDelete { get; set; }
    }
}
