using Microsoft.EntityFrameworkCore;


namespace DekanatDB
{
    [Index("RecordNumber", IsUnique = true)] //настроим также уникальность номера зачётной книжки
    public class Student
    {
        public int Id { get; set; }
        public string LastName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? MiddleName { get; set; }

        public int RecordNumber { get; set; }
        public string? DateOfBirth { get; set; }

        public int FacultyId { get; set; }
        public Faculty? Faculty { get; set; }

    }
}
