using BibliotecaBackend.Data;
using BibliotecaBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var jwtConfig = builder.Configuration.GetSection("Jwt");
var claveSecreta = jwtConfig["Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(claveSecreta)),
        
    };
});


//Add services to the container.
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<ChatWebSocketHandler>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Biblioteca API", Version = "v1" });

    // JWT support en Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Token JWT con esquema: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/auth/helpchat/ws")
    {
        var handler = context.RequestServices.GetRequiredService<ChatWebSocketHandler>();
        await handler.HandleAsync(context);
    }
    else
    {
        await next();
    }
});


app.UseHttpsRedirection();
app.UseAuthentication(); // Antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
