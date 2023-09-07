using Microsoft.EntityFrameworkCore;

namespace DekanatDB
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Faculty> Facultys { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer("Data Source=Students.db");

        }
        */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Faculty>().HasData(
                    new Faculty
                    {
                        Id = 1,
                        NameFaculty = "Машиностроительный"
                    },
                    new Faculty
                    {
                        Id = 2,
                        NameFaculty = "Естественные науки"
                    });
            

            modelBuilder.Entity<Student>().HasData(
                    new Student
                    {
                        Id = 1,
                        LastName = "Иванов",
                        Name = "Иван",
                        MiddleName = "Иванович",
                        RecordNumber = 1805566,
                        DateOfBirth = "17.03.1998",
                        FacultyId = 1
                    },
                    new Student
                    {
                        Id = 2,
                        LastName = "Синицын",
                        Name = "Михаил",
                        MiddleName = "Александрович",
                        RecordNumber = 1805546,
                        DateOfBirth = "05.09.1998",
                        FacultyId = 2
                    });
        }

    }


}
