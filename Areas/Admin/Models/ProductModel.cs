using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Tittle { get; set; }
        [MaxLength(int.MaxValue)]
        public string? Description { get; set; }
        [MaxLength(int.MaxValue)]

        public string Image { get; set; }
        [Range(0, 100)]
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public byte Isdelete { get; set; }
       
        public int? CategoryId { get; set; }
    }
}
