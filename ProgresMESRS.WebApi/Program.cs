using Akavache;
using Akavache.Sqlite3;
using LiteDB.Async;
using ProgresMESRS.Middleware.API;
using ProgresMESRS.WebApi.Service;

var builder = WebApplication.CreateBuilder(args);

BlobCache.LocalMachine = new SqlRawPersistentBlobCache(AppContext.BaseDirectory + "imagecache.db");
Akavache.Registrations.Start("ProgresImageCache");
var db = new LiteDatabaseAsync(@"Filename=D:\ProgresDb.db;Connection=shared");

var browser = new Worker("https://progres.mesrs.dz/webfve");
browser.Page.DownloadHandler = new ProgresDownloadHandler();

ApiClient progresApiClient = new ApiClient();


StudentsListScrapperXml scrapper = new StudentsListScrapperXml();
StudentDetailScrapperXml detailsScrapper = new StudentDetailScrapperXml(browser);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<ILiteDatabaseAsync>(db)
    .AddSingleton(scrapper)
    .AddSingleton(detailsScrapper)
    .AddSingleton(browser)
    .AddSingleton(progresApiClient)
    .AddScoped<IStudentService, StudentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
