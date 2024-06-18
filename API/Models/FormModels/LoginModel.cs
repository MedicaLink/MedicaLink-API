using System.ComponentModel.DataAnnotations;

namespace API.Models.FormModels
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }
    }
}
