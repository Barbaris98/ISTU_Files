using Microsoft.EntityFrameworkCore;

namespace DekanatDB
{
    public class Faculty
    {
        public int Id { get; set; }
        public string? NameFaculty { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}
