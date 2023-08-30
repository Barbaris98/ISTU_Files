var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseStaticFiles(); // добавляем поддержку статических файлов



app.MapGet("/", () => "Hello World!");

app.Run();


