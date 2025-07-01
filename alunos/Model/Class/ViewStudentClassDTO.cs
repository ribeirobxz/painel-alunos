namespace alunos.Model.Class
{
    public class ViewStudentClassDTO
    {
        public Guid Id { get; set; } 
        public string StudentName { get; set; }
        public string DayOfWeek { get; set; }
        public long RemainingTimeInSeconds { get; set; }
    }
}
