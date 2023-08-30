var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseDefaultFiles(); // поддержка страниц html по умолчанию
app.UseStaticFiles(); // добавляем поддержку статических файлов


app.Map("/getFile.png", () =>
{
    string path = "Pic/P1.png";
    string contentType = "image/png";
    string downloadName = "File.png";
    return Results.File(path, contentType, downloadName);
});



app.Run();




