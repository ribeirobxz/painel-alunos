using alunos.Model.Class;
using alunos.Model.Student;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.context
{
    public class ClassDBContext : DbContext
    {

        public ClassDBContext(DbContextOptions<ClassDBContext> options) : base(options) { }

        public DbSet<StudentClass> classes { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
