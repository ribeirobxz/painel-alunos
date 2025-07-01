using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model.Teacher
{
    public class Teacher
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public Teacher(string Name, string PasswordHash)
        {
            this.Name = Name;
            this.PasswordHash = PasswordHash;

            CreatedAt = DateTime.Now;
        }
    }
}
