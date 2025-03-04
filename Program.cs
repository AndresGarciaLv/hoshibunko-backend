
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Hoshibunko.Context;
using hoshibunko.Services.IService;
using hoshibunko.Services.Service;
using hoshibunko.Services.Implementations;
using hoshibunko.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
    .CreateLogger();
builder.Host.UseSerilog();

// 2. Configurar EF Core con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Configurar Identity con la clase Usuario personalizada y Roles
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>();

// 4. Configurar JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]
    ?? throw new InvalidOperationException("JWT Secret no está configurado."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// 5. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 6. Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Definición para poder probar endpoints con token
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// 7. Registrar Controladores y Servicios
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ILibroImagenService, LibroImagenService>();

// Construir la aplicación
var app = builder.Build();

// 8. Mostrar Swagger en modo Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 9. Pipeline de Middlewares
app.UseHttpsRedirection();
app.UseCors("AllowAll"); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

// 10. Crear roles por defecto al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<Usuario>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


    await DbSeeder.SeedAsync(context, userManager, roleManager);
}


// 11. Ejecutar la aplicación
app.Run();

