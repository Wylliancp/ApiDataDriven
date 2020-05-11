using System.ComponentModel.DataAnnotations;

namespace ApiDataDriven.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é Obrigatório")]
        [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Este campo é Obrigatório")]
        [MaxLength(15, ErrorMessage = "Este campo deve conter entre 3 e 15 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 15 caracteres")]
        public string Password { get; set; }
        public string Role { get; set; }

    }
}