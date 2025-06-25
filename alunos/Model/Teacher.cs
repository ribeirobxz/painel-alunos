using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model
{
    public class Teacher
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime createdAt { get; set; }

        public Teacher(string Name, string Password)
        {
            this.Name = Name;
            this.Password = Password;

            this.createdAt = DateTime.Now;
        }
    }
}
