using IRSGenerator.API.Extensions;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Data.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString = builder.Configuration.GetConnectionString("PosgresConnection") ?? "";

builder.Services
    .AddHttpContextAccessor()
    .ConfigureDatabaseContext(dbConnectionString)
    .ConfigureAuthentication()
    .ConfigureAuthorization()
    .ConfigureMapper()
    .ConfigureServices();

// QualiSight repository registrations
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
builder.Services.AddScoped<IDefectRepository, DefectRepository>();
builder.Services.AddScoped<IDefectTypeRepository, DefectTypeRepository>();
builder.Services.AddScoped<IDefectFieldRepository, DefectFieldRepository>();
builder.Services.AddScoped<IDispositionRepository, DispositionRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IVisualSystemConfigRepository, VisualSystemConfigRepository>();

// Yeni QualiSight repository kayıtları
builder.Services.AddScoped<IVisualProjectRepository, VisualProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDispositionTypeRepository, DispositionTypeRepository>();
builder.Services.AddScoped<IDispositionTransitionRepository, DispositionTransitionRepository>();

// IRSGenerator word services
builder.Services.AddScoped<WordOpSheetParser>();
builder.Services.AddScoped<WordReportWriter>();

// IRSGenerator core repository kayıtları
builder.Services.AddScoped<IIRSProjectRepository, IRSProjectRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<INumericPartResultRepository, NumericPartResultRepository>();
builder.Services.AddScoped<ICategoricalPartResultRepository, CategoricalPartResultRepository>();
builder.Services.AddScoped<ICategoricalZoneResultRepository, CategoricalZoneResultRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // snake_case JSON — frontend'le uyumluluk için
        opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// QualiSight UI'yi wwwroot'tan sun (index.html → /)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// SPA fallback — hash routing için (opsiyonel)
app.MapFallbackToFile("index.html");

app.Run();
