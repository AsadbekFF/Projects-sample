using ProjectWithAuthenticationSample.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectWithAuthenticationSample.ViewModels
{
    public class LogInViewModel
    {
        [Display(Name = "Foydalanuvchi")]
        [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
        public string Username { get; set; }

        [Display(Name = "Parol")]
        [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
        [PasswordPropertyText(false)]
        public string Password { get; set; }

        [Display(Name = "Esda qolsin")]
        public bool RememberMe { get; set; }
    }
}
