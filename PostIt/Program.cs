using Microsoft.EntityFrameworkCore;
using PostIt.Data;
using PostIt.Data.Interfaces;
using PostIt.Data.Repositories;
using PostIt.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PostItContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("PostIt.API")));

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PostIt v1"));

app.UseRouting();
app.MapControllers();

app.Run();