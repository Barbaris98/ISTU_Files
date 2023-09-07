using DekanatDB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ ����������� �� ����� ������������
string? connectoin = builder.Configuration.GetConnectionString("DefaultConnection");
// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectoin));

var app = builder.Build();
app.UseDefaultFiles(); // ��������� ������� html �� ���������
app.UseStaticFiles(); // ��������� ��������� ����������� ������


app.Map("/getDB", (ApplicationContext db) =>
{
    db.Students.ToList();

});

// ���������� ���������� �������� � �����
app.Map("/getFile.png", () =>
{
    string path = "Pic/P1.png";
    return Results.File(System.Text.Encoding.UTF8.GetBytes(path),
            "image/png",
            "P1Download.png");

    /*
    string path = "Pic/P1.png";
    byte[] fileContent = await File.ReadAllBytesAsync(path);  // ��������� ���� � ������ ������
    string contentType = "image/png";       // ��������� mime-����
    string downloadName = "P1Download.png";  // ��������� ������������ �����
    return Results.File(fileContent, contentType, downloadName);
    */

});


//��� ����������, ������� ����� ������������ ������� � ������������ � ���� ������:
#region
app.MapGet("/api/students", async (ApplicationContext db) => await db.Students.ToListAsync());

app.MapGet("/api/students/{id:int}", async (int id, ApplicationContext db) =>
{
    // �������� ������������ �� id
    Student? student = await db.Students.FirstOrDefaultAsync(u => u.Id == id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (student == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, ���������� ���
    return Results.Json(student);
});

app.MapDelete("/api/students/{id:int}", async (int id, ApplicationContext db) =>
{
    // �������� ������������ �� id
    Student? student = await db.Students.FirstOrDefaultAsync(u => u.Id == id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (student == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, ������� ���
    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.Json(student);
});

app.MapPost("/api/students", async (Student student, ApplicationContext db) =>
{
    // ��������� ������������ � ������
    await db.Students.AddAsync(student);
    await db.SaveChangesAsync();
    return student;
});

app.MapPut("/api/students", async (Student studentData, ApplicationContext db) =>
{
    // �������� ������������ �� id
    var student = await db.Students.FirstOrDefaultAsync(u => u.Id == studentData.Id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (student == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, �������� ��� ������ � ���������� ������� �������
    student.LastName = studentData.LastName;
    student.Name = studentData.Name;
    student.MiddleName = studentData.MiddleName;
    student.RecordNumber = studentData.RecordNumber;
    student.DateOfBirth = studentData.DateOfBirth;
    student.FacultyId = studentData.FacultyId;
    await db.SaveChangesAsync();
    return Results.Json(student);
});
#endregion



app.Run();




