using API.Application;
using API.Data;
using API.Data.servicesData.services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(cadenaConexion);
});

// Configure CORS for Angular application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IUserRepository, UserData>();
builder.Services.AddScoped<IUsuarioApp,UsuarioApp>();
builder.Services.AddScoped<IPersonaRepository,PersonData>();
builder.Services.AddScoped<IRolRepository,RolData>();
builder.Services.AddScoped<IRoleApp, RoleApp>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddSwaggerGen(c =>
{
    // Puedes agregar configuraciones adicionales aquí si es necesario
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });
});
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS with the policy
app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
