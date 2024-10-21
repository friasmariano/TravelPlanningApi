using System.Security.Claims;
using System.Text;
using Isopoh.Cryptography.Argon2;
using TravelPlanningApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelPlanningApi.Interfaces;
using TravelPlanningApi.Models;
using Microsoft.OpenApi.Models;
using TravelPlanningApi.Repositories;
using TravelPlanningApi.Services;
using FluentValidation;
using TravelPlanningApi.Validators;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registering the DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adding authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin_policy", policy => policy.RequireRole("admin"));

// Swagger Authorization
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Biblioteca Api", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IUserValidationService, UserValidationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<EmailValidator>();
builder.Services.AddScoped<PasswordValidator>();
builder.Services.AddScoped<PasswordHashValidator>();

builder.Services.AddControllers();

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

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

#region UsuariosPorDefecto

using (var scope = app.Services.CreateScope())
{
    var roles = new[] { "Admin", "Traveler" };
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>(); 
    await SeedUsersAsync(context);
}

async Task<IActionResult> CreateUserIfNotExists(string email, string password, string role, 
                          string personaName, string userName, ApplicationDbContext context) {
    
    var usuario = await context.Usuarios
                        .Where(r => r.Email == email)
                        .FirstOrDefaultAsync();

    if (usuario != null)
    {
        return new BadRequestObjectResult(new { message = "El usuario ya existe." });  
    }

    var person = await context.Personas
                               .Where(r => r.Nombre == personaName)
                               .FirstOrDefaultAsync();
    
    var persona = new Persona() { Nombre = personaName };
    await context.Personas.AddAsync(persona);
    await context.SaveChangesAsync();
        
    var newRole = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == role);

    if (newRole != null) {
        return new BadRequestObjectResult(new { message = "El rol ya existe." });
    }
    
    var rolTemp = new Rol() { Nombre = role };
    context.Roles.Add(rolTemp);
    await context.SaveChangesAsync();
    
    DateTime now = DateTime.Now;
    var user = new Usuario()
    {
        Username = userName,
        PersonaId = persona.Id,
        Email = email,
        Password = Argon2.Hash(password),
        ProfilePic = "ND",
        FechaCreacion = now
    };
    context.Usuarios.Add(user);
    await context.SaveChangesAsync();

    var userRol = new UsuarioRol()
    {
        UsuarioId = user.Id,
        RolId = rolTemp.Id
    };
    context.UsuarioRoles.Add(userRol);
    await context.SaveChangesAsync();

    return new OkObjectResult("El usuario ha sido creado con éxito.");
}

async Task SeedUsersAsync(ApplicationDbContext context)
{
    var adminUser = await CreateUserIfNotExists("admin@travelp.com", "Qwerty12345*", "Admin", "Administrator", "admin", context);
    var travelerUser = await CreateUserIfNotExists("john.smith@travelp.com", "Qwerty12345*", "Traveler", "John Smith", "john.smith", context);
}
#endregion

app.MapControllers();

app.Run();