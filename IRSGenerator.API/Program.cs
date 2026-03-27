using IRSGenerator.API.Extensions;
using IRSGenerator.API.Services;
using IRSGenerator.API.Utils;
using IRSGenerator.Core;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Data;
using IRSGenerator.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
