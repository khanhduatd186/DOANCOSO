using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(100)]
#pragma warning disable CS8618 // Non-nullable property 'Tittle' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public string Tittle { get; set; }
#pragma warning restore CS8618 // Non-nullable property 'Tittle' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        [MaxLength(int.MaxValue)]
        public string? Description { get; set; }
        [MaxLength(int.MaxValue)]
#pragma warning disable CS8618 // Non-nullable property 'Image' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public string Image { get; set; }
#pragma warning restore CS8618 // Non-nullable property 'Image' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        [Range(0, 100)]
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        public byte Isdelete { get; set; }
        public string NameCategory { get; set; }
    }
}
