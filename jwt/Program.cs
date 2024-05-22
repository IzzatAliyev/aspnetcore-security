// Copyright (c) IUA. All rights reserved.

namespace jwt;

using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using jwt.Data;
using jwt.Models;
using jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

/// <summary>
/// Starting class.
/// </summary>
public class Program
{
    /// <summary>
    /// Starting main method.
    /// </summary>
    /// <param name="args">the arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Jwt API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer",
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    []
                },
            });
        });

        builder.Services.AddProblemDetails();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddScoped<TokenService, TokenService>();

        builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseMySQL(builder.Configuration.GetConnectionString("DB") ?? throw new Exception("connection string doesn't exist"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        var validIssuer = builder.Configuration.GetValue<string>("TokenSettings:ValidIssuer");
        var validAudience = builder.Configuration.GetValue<string>("TokenSettings:ValidAudience");
        var symmetricSecurityKey = builder.Configuration.GetValue<string>("TokenSettings:SymmetricSecurityKey") ?? "defaultKey";

        builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                    {
                        options.IncludeErrorDetails = true;
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ClockSkew = TimeSpan.Zero,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = validIssuer,
                            ValidAudience = validAudience,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(symmetricSecurityKey)),
                        };
                    });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseStatusCodePages();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
