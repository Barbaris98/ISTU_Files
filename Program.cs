var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseDefaultFiles(); // ��������� ������� html �� ���������
app.UseStaticFiles(); // ��������� ��������� ����������� ������


app.Map("/getFile.png", () =>
{
    string path = "Pic/P1.png";
    string contentType = "image/png";
    string downloadName = "File.png";
    return Results.File(path, contentType, downloadName);
});



app.Run();




