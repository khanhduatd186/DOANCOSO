using System.ComponentModel.DataAnnotations;

namespace ApiPetShop.Models
{
    public class UpdatePermissionModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } 
      
    }
}
