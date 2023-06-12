using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Models
{
    public class SignInModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        public string PassWord { get; set; } = null!;
    }
   
}
