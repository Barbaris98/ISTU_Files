using DekanatDB;
using ISTU_Files;
using Microsoft.AspNetCore.Identity;
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
    db.Users.ToList();

});

// ���������� ���������� �������� � �����
app.Map("/getFile", () =>
{
    string path = "wwwroot/Pic/P1.png";
    
    return Results.File(System.Text.Encoding.UTF8.GetBytes(path),
            "image/png",
            "P1Load.png");
    
    /*
    FileStream fileStream = new FileStream(path, FileMode.Open);
    string contentType = "image/png";
    string downloadName = "ThePic.png";
    return Results.File(fileStream, contentType, downloadName);
    */

    /*
    string path = "Pic/P1.png";
    byte[] fileContent = await File.ReadAllBytesAsync(path);  // ��������� ���� � ������ ������
    string contentType = "image/png";       // ��������� mime-����
    string downloadName = "P1Download.png";  // ��������� ������������ �����
    return Results.File(fileContent, contentType, downloadName);
    */

});



#region ��� ����������, ������� ����� ������������ ������� � ������������ � ���� ������:
app.MapGet("/api/users", async (ApplicationContext db) => await db.Users.ToListAsync());

app.MapGet("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // �������� ������������ �� id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (user == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, ���������� ���
    return Results.Json(user);
});

app.MapDelete("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // �������� ������������ �� id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (user == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, ������� ���
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/users", async (User user, ApplicationContext db) =>
{
    // ��������� ������������ � ������
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});

app.MapPut("/api/users", async (User userData, ApplicationContext db) =>
{
    // �������� ������������ �� id
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userData.Id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (user == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, �������� ��� ������ � ���������� ������� �������
    user.LastName = userData.LastName;
    user.Name = userData.Name;
    user.MiddleName = userData.MiddleName;
    user.RecordNumber = userData.RecordNumber;
    user.DateOfBirth = userData.DateOfBirth;

    await db.SaveChangesAsync();
    return Results.Json(user);
});
#endregion



app.Run();




