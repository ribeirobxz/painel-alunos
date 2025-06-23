using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;

namespace WebApplication3.context
{
    public class ClassDBContext : DbContext
    {

        public ClassDBContext(DbContextOptions<ClassDBContext> options) : base(options) { }

        public DbSet<StudentClass> classes { get; set; }
    }
}
