using DekanatDB;
using ISTU_Files;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System;

var builder = WebApplication.CreateBuilder(args);

// �������� �� � ��� ��������������
var people = new List<Person>
{
    new Person("Student", "12345"),
    new Person("Admin", "12345ADM")
};

// �������������� � ������� ����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

// �������� ������ ����������� �� ����� ������������
string? connectoin = builder.Configuration.GetConnectionString("DefaultConnection");
// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectoin));

var app = builder.Build();
app.UseDefaultFiles(); // ��������� ������� html �� ���������
app.UseStaticFiles(); // ��������� ��������� ����������� ������

app.UseAuthentication();   // ���������� middleware �������������� 
app.UseAuthorization();   // ���������� middleware ����������� 

// ���������� ���������� �������� � �����
app.Map("/getFileMetodicka", () =>
{
    string path = "wwwroot/Pdf/Metod.pdf";
    FileStream fileStream = new FileStream(path, FileMode.Open);
    string contentType = "pdf/pdf";
    string downloadName = "Metodicheskoe_posobie_po_ispytaniam_SPV.pdf";
    return Results.File(fileStream, contentType, downloadName);

});
app.Map("/getFile����50529", () =>
{
	string path = "wwwroot/Pdf/����50529.pdf";
	FileStream fileStream = new FileStream(path, FileMode.Open);
	string contentType = "pdf/pdf";
	string downloadName = "����50529.pdf";
	return Results.File(fileStream, contentType, downloadName);

});
app.Map("/getFile����50530", () =>
{
	string path = "wwwroot/Pdf/����50530.pdf";
	FileStream fileStream = new FileStream(path, FileMode.Open);
	string contentType = "pdf/pdf";
	string downloadName = "����50530.pdf";
	return Results.File(fileStream, contentType, downloadName);

});
app.Map("/getFile������������������", () =>
{
	string path = "wwwroot/Word/������������������.docx";
	FileStream fileStream = new FileStream(path, FileMode.Open);
	string contentType = "docx/docx";
	string downloadName = "������������������.docx";
	return Results.File(fileStream, contentType, downloadName);

});

#region  ��� ����������, ������� ����� ������������ ������� � ������������ � ���� ������:
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

#region ��� ��������������
app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    // html-����� ��� ����� ������/������
    string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>METANIT.COM</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <h4>���������� ��� �������, �� ��������� 'Student'-'12345' ��� ��� Admin 'Admin'-'12345ADM'<h4>
        <form method='post'>
            <p>
                <label>Name</label><br />
                <input name='name' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <input type='submit' value='Login' />
        </form>
    </body>
    </html>";
    await context.Response.WriteAsync(loginForm);
});

app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    // �������� �� ����� name � ������
    var form = context.Request.Form;
    // ���� name �/��� ������ �� �����������, �������� ��������� ��� ������ 400
    if (!form.ContainsKey("name") || !form.ContainsKey("password"))
        return Results.BadRequest("Name �/��� ������ �� �����������");

    string? name = form["name"];
    string? password = form["password"];

    // ������� ������������ 
    Person? person = people.FirstOrDefault(p => p.Name == name && p.Password == password);
    // ���� ������������ �� ������, ���������� ��������� ��� 401
    if (person is null) return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Name) };
    // ������� ������ ClaimsIdentity
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // ��������� ������������������ ����
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

    return Results.Redirect(returnUrl ?? "/AuthenticationSuccessful.html");
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});
#endregion

app.Run();

record class Person(string Name, string Password);
