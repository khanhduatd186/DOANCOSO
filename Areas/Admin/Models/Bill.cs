using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class Bill
    {
        public int Id { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public DateTimeOffset dateTime { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public byte IsDelete { get; set; }
    }
}
