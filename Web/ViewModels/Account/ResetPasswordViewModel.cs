using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
