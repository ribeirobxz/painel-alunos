using alunos.Model.Class;
using alunos.Model.Course;
using alunos.Model.Students;
using alunos.Model.Teacher;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace WebApplication3.context
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<StudentClass> classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseClass> CourseClasses { get; set; }
        public DbSet<CourseClassStep> CourseClassSteps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=migrations_temp.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(builder =>
            {
                builder.Property(s => s.CoursesClass).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, (JsonSerializerOptions)null)
                );
                builder.Property(s => s.CoursesClassStep).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, (JsonSerializerOptions)null)
                );
            });
        }
    }
}
