namespace alunos.Model.Students
{
    public class CreateStudentDTO
    {

        public string name { get; set; }
        public string[] daysOfWeek { get; set; }
        public int[] courses { get; set; }
    }
}
