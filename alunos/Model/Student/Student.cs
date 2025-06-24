using System.ComponentModel.DataAnnotations;

namespace alunos.Model.Student
{
    public class Student
    {

        [Key]
        public string name { get; set; }
        public string dayOfWeek { get; set; }
        public DateTime registeredAt { get; set; }

        public Student(string name, string dayOfWeek)
        {
            this.name = name;
            this.dayOfWeek = dayOfWeek;
            this.registeredAt = DateTime.Now;
        }

    }
}
