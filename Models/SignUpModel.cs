using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Models
{
    public class SignUpModel
    {
        public string? id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;
        [Required]
        public string Adddress { get; set; } = null!;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        public string PassWord { get; set; } = null!;
        [Required]
        public string ConfirmPassWord { get; set; } = null!;
    }
   
}
