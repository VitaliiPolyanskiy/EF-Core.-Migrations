using Microsoft.EntityFrameworkCore;
using StudentLibrary;

namespace AcademyGroupContextLib
{
    public class AcademyGroupContext : DbContext
    {
        public AcademyGroupContext()
        {
            //Database.EnsureCreated(); // При виконанні міграції цей метод викликає помилку.
        }

        public DbSet<AcademyGroup> AcademyGroups { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Метод UseLazyLoadingProxies() робить доступною ліниве завантаження.
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=AcademyGroupMigrations;Integrated Security=SSPI;TrustServerCertificate=true");
        }
    }
}