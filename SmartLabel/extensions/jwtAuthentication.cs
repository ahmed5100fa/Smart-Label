using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace SmartLabel.extensions
{
    public static class jwtAuthentication
    {
        public static void addjwtAuthentictaion( this IServiceCollection service,ConfigurationManager configuration)
        {
            service.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme =JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = false;
                op.SaveToken = true;
                op.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["jwt:issuer"],
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:secretKey"]))
                };

            }); 
        }
        public static void swagerJwtAuthenticaion(this IServiceCollection service)
        {
            service.AddSwaggerGen(op =>
            {
                op.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "smart lapel",
                    Description = "project to show the products price"
                });
                op.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat ="jwt",
                    In = ParameterLocation.Header,
                    Description = "enter the jwt token"

                });
                op.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Name = "Bearer",
            In = ParameterLocation.Header
        },
        new List<string>()
    }
});
            }
           
            );

            
        }

    }
}
