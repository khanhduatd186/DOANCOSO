
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class BillModel
    {
       
        public int Id { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dateTime { get; set; }
        
        public int Status { get; set; }
        public string? IdUser { get; set; }
    }
}
