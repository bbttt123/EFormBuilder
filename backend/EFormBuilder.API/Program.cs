using EFormBuilder.API.Middlewares;
using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Application.Services;
using EFormBuilder.Infrastructure.Persistence;
using EFormBuilder.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Services (DbContext, Controllers, Swagger, DI...)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new ValidationError
                {
                    Field = e.Key,
                    Message = e.Value!.Errors.First().ErrorMessage,
                    Annotation = "Validation"
                }).ToList();

            var response = new ApiResponse<object>
            {
                Success = false,
                Code = "VAL_001",
                Message = "Validation failed",
                Errors = errors,
                Timestamp = DateTime.UtcNow,
                Path = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EFormBuilder.API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    });

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
builder.Services.AddHttpContextAccessor(); // IHttpContextAccessor
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IPublicFormService, PublicFormService>();
builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:8080") // Cổng của Vite
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Rất quan trọng vì bạn có sài Cookie
    });
});

// 2. Cấu hình JWT & Custom Response cho 401/403
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

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

    // THÊM ĐOẠN NÀY ĐỂ BẮT LỖI 401 (Giống InsufficientAuthenticationException bên Java)
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            var response = ApiResponse<object>.Failure("SEC_001", "Chưa đăng nhập hoặc Token hết hạn");
            await context.Response.WriteAsJsonAsync(response);
        },
        OnForbidden = async context => // THÊM DÒNG NÀY
        {
            context.Response.StatusCode = 403;
            var response = ApiResponse<object>.Failure("SEC_002", "Bạn không có quyền truy cập tài nguyên này");
            await context.Response.WriteAsJsonAsync(response);
        }
    };
});


var app = builder.Build();


// PHẢI ĐỂ ĐẦU TIÊN để hứng toàn bộ lỗi từ các lớp bên dưới
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authentication phải đứng TRƯỚC Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.Failure("SYS_404", "Đường dẫn (Route) không tồn tại");
        await context.Response.WriteAsJsonAsync(response);
    }
});

app.Run();