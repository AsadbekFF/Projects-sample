using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ProjectWithAuthenticationSample.Infrastructure;

namespace ProjectWithAuthenticationSample.ViewModels
{
    public class UserViewModel
    {
        [Display(Name = "Foydalanuvchi")]
        [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
        public string Username { get; set; }

        [Display(Name = "Ismi")]
        [MaxLength(255, ErrorMessage = DataAnnotationsResources.MaxLengthAttribute_ValidationError)]
        public string? FirstName { get; set; }

        [Display(Name = "Familiyasi")]
        [MaxLength(255, ErrorMessage = DataAnnotationsResources.MaxLengthAttribute_ValidationError)]
        public string? Lastname { get; set; }

        [Display(Name = "Parol")]
        [Required(ErrorMessage = DataAnnotationsResources.RequiredAttribute_ValidationError)]
        [MaxLength(255, ErrorMessage = DataAnnotationsResources.MaxLengthAttribute_ValidationError)]
        [PasswordPropertyText(false)]
        public string Password { get; set; }
    }
}
