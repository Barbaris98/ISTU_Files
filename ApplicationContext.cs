using ISTU_Files;
using Microsoft.EntityFrameworkCore;

namespace DekanatDB
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        Id = 1,
                        LastName = "Александров",
                        Name = "Никита",
                        MiddleName = "Сергеевич",
                        RecordNumber = 18051351,
                        DateOfBirth = "08.31.1998"
                    },
                    new User
                    {
                        Id = 2,
                        LastName = "Беляев",
                        Name = "Антон",
                        MiddleName = "Сергеевич",
                        RecordNumber = 18051301,
                        DateOfBirth = "21.07.1999"
                    },
                    new User
                    {
                        Id = 3,
                        LastName = "Брик",
                        Name = "Валериий",
                        MiddleName = "Романович",
                        RecordNumber = 18051355,
                        DateOfBirth = "02.04.2000"
                    },
                    new User
                    {
                        Id = 4,
                        LastName = "Буров",
                        Name = "Андрей",
                        MiddleName = "Николаевич",
                        RecordNumber = 18051358,
                        DateOfBirth = "17.12.1998"
                    },
                    new User
                    {
                        Id = 5,
                        LastName = "Камашев",
                        Name = "Григорий",
                        MiddleName = "Андреевич",
                        RecordNumber = 18051400,
                        DateOfBirth = "04.04.1998"
                    },
                    new User
                    {
                        Id = 6,
                        LastName = "Кузнецов",
                        Name = "Дмитрий",
                        MiddleName = "Андреевич",
                        RecordNumber = 18051372,
                        DateOfBirth = "21.08.19998"
                    },
                    new User
                    {
                        Id = 7,
                        LastName = "Новиков",
                        Name = "Максим",
                        MiddleName = "Александрович",
                        RecordNumber = 18051376,
                        DateOfBirth = "18.03.1998"
                    },
                    new User
                    {
                        Id = 8,
                        LastName = "Яркеева",
                        Name = "Камила",
                        MiddleName = "Сергеевна",
                        RecordNumber = 18051486,
                        DateOfBirth = "22.12.1999"
                    }
            );
        }
    }


}
