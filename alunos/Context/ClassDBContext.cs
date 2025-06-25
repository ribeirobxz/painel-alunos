using alunos.Model;
using alunos.Model.Class;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.context
{
    public class ClassDBContext : DbContext
    {

        public ClassDBContext(DbContextOptions<ClassDBContext> options) : base(options) { }

        public DbSet<StudentClass> classes { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Se o DbContext ainda não foi configurado (o que acontece quando 'dotnet ef' o chama),
            // configure-o para usar SQLite.
            if (!optionsBuilder.IsConfigured)
            {
                // Isso cria um arquivo de banco de dados SQLite na pasta do projeto.
                optionsBuilder.UseSqlite("DataSource=migrations_temp.db");
            }
        }
    }
}
