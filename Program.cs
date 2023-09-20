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

// условная бд с для аутентификация
var people = new List<Person>
{
    new Person("Student", "12345"),
    new Person("Admin", "12345ADM")
};

// аутентификация с помощью куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

// получаем строку подключения из файла конфигурации
string? connectoin = builder.Configuration.GetConnectionString("DefaultConnection");
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectoin));

var app = builder.Build();
app.UseDefaultFiles(); // поддержка страниц html по умолчанию
app.UseStaticFiles(); // добавляем поддержку статических файлов

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 

// функционал скачивания ресурсов с сайта
app.Map("/getFileMetodicka", () =>
{
    string path = "wwwroot/Pdf/Metod.pdf";
    FileStream fileStream = new FileStream(path, FileMode.Open);
    string contentType = "pdf/pdf";
    string downloadName = "Metodicheskoe_posobie_po_ispytaniam_SPV.pdf";
    return Results.File(fileStream, contentType, downloadName);

});
app.Map("/getFileГОСТ50529", () =>
{
	string path = "wwwroot/Pdf/ГОСТ50529.pdf";
	FileStream fileStream = new FileStream(path, FileMode.Open);
	string contentType = "pdf/pdf";
	string downloadName = "ГОСТ50529.pdf";
	return Results.File(fileStream, contentType, downloadName);

});
app.Map("/getFileГОСТ50530", () =>
{
	string path = "wwwroot/Pdf/ГОСТ50530.pdf";
	FileStream fileStream = new FileStream(path, FileMode.Open);
	string contentType = "pdf/pdf";
	string downloadName = "ГОСТ50530.pdf";
	return Results.File(fileStream, contentType, downloadName);

});
app.Map("/getFileПрактическаяРабота", () =>
{
	string path = "wwwroot/Word/ПрактическаяРабота.docx";
	FileStream fileStream = new FileStream(path, FileMode.Open);
	string contentType = "docx/docx";
	string downloadName = "ПрактическаяРабота.docx";
	return Results.File(fileStream, contentType, downloadName);

});

#region  Код приложения, который будет обрабатывать запросы и подключаться к базе данных:
app.MapGet("/api/users", async (ApplicationContext db) => await db.Users.ToListAsync());

app.MapGet("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, отправляем его
    return Results.Json(user);
});

app.MapDelete("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, удаляем его
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/users", async (User user, ApplicationContext db) =>
{
    // добавляем пользователя в массив
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});

app.MapPut("/api/users", async (User userData, ApplicationContext db) =>
{
    // получаем пользователя по id
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userData.Id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
    user.LastName = userData.LastName;
    user.Name = userData.Name;
    user.MiddleName = userData.MiddleName;
    user.RecordNumber = userData.RecordNumber;
    user.DateOfBirth = userData.DateOfBirth;

    await db.SaveChangesAsync();
    return Results.Json(user);
});
#endregion

#region Код аутентификации
app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    // html-форма для ввода логина/пароля
    string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>METANIT.COM</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <h4>Продолжить как Студент, по умолчанию 'Student'-'12345' или как Admin 'Admin'-'12345ADM'<h4>
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
    // получаем из формы name и пароль
    var form = context.Request.Form;
    // если name и/или пароль не установлены, посылаем статусный код ошибки 400
    if (!form.ContainsKey("name") || !form.ContainsKey("password"))
        return Results.BadRequest("Name и/или пароль не установлены");

    string? name = form["name"];
    string? password = form["password"];

    // находим пользователя 
    Person? person = people.FirstOrDefault(p => p.Name == name && p.Password == password);
    // если пользователь не найден, отправляем статусный код 401
    if (person is null) return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Name) };
    // создаем объект ClaimsIdentity
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // установка аутентификационных куки
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
