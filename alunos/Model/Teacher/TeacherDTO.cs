namespace alunos.Model.Teacher
{
    public class TeacherDTO
    {

        public string Name { get; set; }
        public DateTime RegisteredAt {  get; set; }

        public TeacherDTO(string Name, DateTime RegisteredAt)
        {
            this.Name = Name;
            this.RegisteredAt = RegisteredAt;
        }
    }
}
