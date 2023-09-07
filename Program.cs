using DekanatDB;
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
    db.Students.ToList();

});

// функционал скачивания ресурсов с сайта
app.Map("/getFile.png", () =>
{
    string path = "Pic/P1.png";
    return Results.File(System.Text.Encoding.UTF8.GetBytes(path),
            "image/png",
            "P1Download.png");

    /*
    string path = "Pic/P1.png";
    byte[] fileContent = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
    string contentType = "image/png";       // установка mime-типа
    string downloadName = "P1Download.png";  // установка загружаемого имени
    return Results.File(fileContent, contentType, downloadName);
    */

});


//код приложения, который будет обрабатывать запросы и подключаться к базе данных:
#region
app.MapGet("/api/students", async (ApplicationContext db) => await db.Students.ToListAsync());

app.MapGet("/api/students/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    Student? student = await db.Students.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (student == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, отправляем его
    return Results.Json(student);
});

app.MapDelete("/api/students/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    Student? student = await db.Students.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (student == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, удаляем его
    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.Json(student);
});

app.MapPost("/api/students", async (Student student, ApplicationContext db) =>
{
    // добавляем пользователя в массив
    await db.Students.AddAsync(student);
    await db.SaveChangesAsync();
    return student;
});

app.MapPut("/api/students", async (Student studentData, ApplicationContext db) =>
{
    // получаем пользователя по id
    var student = await db.Students.FirstOrDefaultAsync(u => u.Id == studentData.Id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (student == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
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




