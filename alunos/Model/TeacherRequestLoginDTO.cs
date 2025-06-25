using System.ComponentModel.DataAnnotations;

namespace alunos.Model
{
    public class TeacherRequestLoginDTO
    {

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public string name { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório.")]
        public string password { get; set; }
    }
}
