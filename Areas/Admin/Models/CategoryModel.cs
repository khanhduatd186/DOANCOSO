using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(int.MaxValue)]
#pragma warning disable CS8618 // Non-nullable property 'Tittle' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public string Tittle { get; set; }
#pragma warning restore CS8618 // Non-nullable property 'Tittle' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
    }
}
