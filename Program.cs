var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseDefaultFiles(); // поддержка страниц html по умолчанию
app.UseStaticFiles(); // добавляем поддержку статических файлов



app.Run(async (context) => await context.Response.WriteAsync("Hello World"));

app.Run();


