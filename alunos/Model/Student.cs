using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model
{
    public class Student
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string[] DaysOfWeek { get; set; }

        public DateTime RegisteredAt { get; set; }

        public Student() { }

        public Student(string name, string[] daysOfWeek)
        {
            Name = name;
            DaysOfWeek = daysOfWeek;
            RegisteredAt = DateTime.Now;
        }
    }
}