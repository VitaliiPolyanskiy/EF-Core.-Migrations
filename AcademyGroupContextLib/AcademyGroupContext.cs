using Microsoft.EntityFrameworkCore;
using StudentLibrary;

namespace AcademyGroupContextLib
{
    public class AcademyGroupContext : DbContext
    {
        public AcademyGroupContext()
        {
            //Database.EnsureCreated(); //при выполнении миграции этот метод вызывает ошибку
        }

        public DbSet<AcademyGroup> AcademyGroups { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // метод UseLazyLoadingProxies() делает доступной ленивую загрузку.
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Server=DESKTOP-G30VB0K\MSSQLSERVER01;Database=AcademyGroupMigrations;Integrated Security=SSPI;TrustServerCertificate=true");
        }
    }
}