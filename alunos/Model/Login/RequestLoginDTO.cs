using System.ComponentModel.DataAnnotations;

namespace alunos.Model.Login
{
    public class RequestLoginDTO
    {

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório.")]
        public string Password { get; set; }
    }
}
