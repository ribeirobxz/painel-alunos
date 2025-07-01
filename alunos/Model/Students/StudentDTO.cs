using System.ComponentModel.DataAnnotations;

namespace alunos.Model.Students
{
    public class StudentDTO
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string[] DaysOfWeek { get; set; }
        public int[] Courses { get; set; }
        public Dictionary<int, int> CoursesClass { get; set; }
        public Dictionary<int, int> CoursesClassStep { get; set; }

        public DateTime RegisteredAt { get; set; }

        public StudentDTO(Guid id, string name, string[] daysOfWeek, int[] courses, Dictionary<int, int> coursesClass, Dictionary<int, int> coursesClassStep, DateTime registeredAt)
        {
            Id = id;
            Name = name;
            DaysOfWeek = daysOfWeek;
            Courses = courses;
            CoursesClass = coursesClass;
            CoursesClassStep = coursesClassStep;
            RegisteredAt = registeredAt;
        }

    }
}
