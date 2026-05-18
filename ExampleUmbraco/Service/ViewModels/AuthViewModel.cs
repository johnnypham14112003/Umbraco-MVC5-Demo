using System.ComponentModel.DataAnnotations;

namespace Service.ViewModels
{
    public class AuthViewModel
    {
        [Required(ErrorMessage = "Please input email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please input password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}