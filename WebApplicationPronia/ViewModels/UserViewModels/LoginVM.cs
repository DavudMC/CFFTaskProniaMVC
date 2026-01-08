using System.ComponentModel.DataAnnotations;

namespace WebApplicationPronia.ViewModels.UserViewModels
{
    public class LoginVM
    {
        [Required, MaxLength(256), EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        [Required, MaxLength(256), MinLength(8), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool IsRemember { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
