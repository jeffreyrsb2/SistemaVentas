using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Aplicacion.Servicios;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Infraestructura.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Obtenemos la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Definimos CORS hacia el front-end
builder.Services.AddCors(options => {
    options.AddPolicy("AllowWebApp", policy => {
        policy.WithOrigins("https://localhost:7082")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registramos los Repositorios (Capa Infraestructura)
builder.Services.AddScoped<IProductoRepository>(provider => new ProductoRepository(connectionString));
// Registramos los Servicios (Capa Aplicación)
builder.Services.AddScoped<IProductoService, ProductoService>();

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

// Usamos la política de CORS definida anteriormente
app.UseCors("AllowWebApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
