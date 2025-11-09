using Application.Services;
using Domain.Interfaces;
using Domain.RepositoryInterfaces;
using Infrastructure.Auth;
using Infrastructure.Data;
using Infrastructure.Messaging; // ✅ for RabbitMQ
using Infrastructure.RepositoryImplamantations;
using Infrastructure.RepositoryImplementations;
using Infrastructure.RepositoryInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecipeApp.API.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// 🧩 DATABASE CONFIGURATION (PostgreSQL)
// ===========================================
builder.Services.AddDbContext<AppDbcontext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===========================================
// 🧩 REPOSITORY REGISTRATION
// ===========================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<INutritionRepository, NutritionRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ===========================================
// 🧩 SERVICE REGISTRATION
// ===========================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRecipeService, RecipesService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IRatingService, RatingService>();

// ===========================================
// 📨 RABBITMQ REGISTRATION
// ===========================================
// Register RabbitMqPublisher as a singleton to reuse the same connection
builder.Services.AddSingleton<RabbitMqPublisher>();

// ===========================================
// 🌐 ENABLE CORS (important for Swagger and Frontend)
// ===========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ===========================================
// 🔐 JWT AUTHENTICATION CONFIGURATION
// ===========================================
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSection["SecretKey"];
var expiryMinutes = double.Parse(jwtSection["ExpiryMinutes"] ?? "180");

builder.Services.AddSingleton(new JwtService(secretKey!, expiryMinutes));

var key = Encoding.ASCII.GetBytes(secretKey!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ===========================================
// 🧾 CONTROLLERS + SWAGGER (✅ JWT ENABLED)
// ===========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RecipeApp API",
        Version = "v1",
        Description = "API for managing recipes, ingredients, and users"
    });

    // ✅ JWT Authorization in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer eyJhbGciOiJIUzI1NiIs..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ===========================================
// 🚀 BUILD & PIPELINE
// ===========================================
var app = builder.Build();

// ✅ Enable Swagger UI (for development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Enable Static Files (for image access in /wwwroot/images)
app.UseStaticFiles();

// ✅ Enable CORS
app.UseCors("AllowAll");

// ✅ Custom middleware for image validation
app.UseImageValidation();

// ✅ Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers
app.MapControllers();

// ✅ Run Application
app.Run();
