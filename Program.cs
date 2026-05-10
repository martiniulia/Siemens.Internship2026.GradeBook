using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Repositories;
using Siemens.Internship2026.GradeBook.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddHttpClient();

builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeService, GradeService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Content(@"
    <html>
        <body style='font-family: sans-serif; padding: 50px; line-height: 1.6;'>
            <h1>Siemens GradeBook API</h1>
            <p>Use the links below to test the endpoints:</p>
            <ul>
                <li><a href='/api/grade'>All Grades & Statistics</a></li>
                <li>
                    Filter Passing Grades (N): 
                    <input type='number' id='nCount' value='5' style='width: 50px;' />
                    <button onclick=""window.location.href='/api/grade/passing/' + document.getElementById('nCount').value"">Filter</button>
                </li>
            </ul>
        </body>
    </html>", "text/html"));

app.Run();
