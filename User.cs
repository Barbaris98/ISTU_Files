namespace ISTU_Files
{
    public class User
    {
        public int Id { get; set; }
        public string LastName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? MiddleName { get; set; }

        public int RecordNumber { get; set; }
        public string? DateOfBirth { get; set; }
    }
}
