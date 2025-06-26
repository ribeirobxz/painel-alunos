using System.ComponentModel.DataAnnotations;

namespace alunos.Model.Teacher
{
    public class TeacherRequestLoginDTO
    {

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório.")]
        public string Password { get; set; }
    }
}
