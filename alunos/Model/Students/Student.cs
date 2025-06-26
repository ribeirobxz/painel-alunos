using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model.Students
{
    public class Student
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string[] DaysOfWeek { get; set; }
        public int[] Courses { get; set; }
        public Dictionary<int, int> CoursesClass { get; set; }
        public Dictionary<int, int> CoursesClassStep { get; set; }

        public DateTime RegisteredAt { get; set; }

        public Student() { }

        public Student(string name, string[] daysOfWeek)
        {
            Name = name;
            DaysOfWeek = daysOfWeek;
            Courses = new int[100];

            CoursesClass = new Dictionary<int, int>();
            CoursesClassStep = new Dictionary<int, int>();
            RegisteredAt = DateTime.Now;
        }
    }
}