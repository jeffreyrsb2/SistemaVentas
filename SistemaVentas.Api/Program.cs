using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SistemaVentas.Aplicacion.Interfaces;
using SistemaVentas.Aplicacion.Servicios;
using SistemaVentas.Dominio.Interfaces;
using SistemaVentas.Infraestructura.Repositorios;
using System.Text;

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

// --- REGISTRO DE SERVICIOS DE AUTENTICACIÓN ---
builder.Services.AddScoped<IUsuarioRepository>(provider => new UsuarioRepository(connectionString));
builder.Services.AddScoped<IAuthService, AuthService>();

// Registramos el servicio de generación de tokens JWT
builder.Services.AddSingleton<ITokenService>(provider =>
{
    var key = builder.Configuration["Jwt:Key"];
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    // Creamos la instancia del servicio pasándole los strings
    return new TokenService(key, issuer, audience);
});

// --- CONFIGURACIÓN DE JWT BEARER ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Registramos los Repositorios (Capa Infraestructura)
builder.Services.AddScoped<IProductoRepository>(provider => new ProductoRepository(connectionString));
// Registramos los Servicios (Capa Aplicación)
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// Configuración de Swagger para incluir el esquema de seguridad JWT
builder.Services.AddSwaggerGen(options =>
{
    // Define el esquema de seguridad (Bearer Token)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, introduce 'Bearer' seguido de un espacio y el token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Añade el requisito de seguridad a nivel global para que aparezca el candado de seguridad en Swagger UI
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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

// Habilitamos la autenticación
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
