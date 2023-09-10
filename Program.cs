using DekanatDB;
using ISTU_Files;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// получаем строку подключения из файла конфигурации
string? connectoin = builder.Configuration.GetConnectionString("DefaultConnection");
// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectoin));

var app = builder.Build();
app.UseDefaultFiles(); // поддержка страниц html по умолчанию
app.UseStaticFiles(); // добавляем поддержку статических файлов


app.Map("/getDB", (ApplicationContext db) =>
{
    db.Users.ToList();

});

// функционал скачивания ресурсов с сайта
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
    byte[] fileContent = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
    string contentType = "image/png";       // установка mime-типа
    string downloadName = "P1Download.png";  // установка загружаемого имени
    return Results.File(fileContent, contentType, downloadName);
    */

});



#region код приложения, который будет обрабатывать запросы и подключаться к базе данных:
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



app.Run();




