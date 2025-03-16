
using FluentValidation.AspNetCore;
using FluentValidation;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth;
using System.Reflection;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt;
using Microsoft.AspNetCore.Mvc;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using Microsoft.OpenApi.Models;

namespace RecipeDormAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration.AddJsonFile("appsettings.json", true);
            builder.Services.RegisterCors(builder.Configuration);
            builder.Services.RegisterAppSettings(builder.Configuration);
            builder.Services.RegisterPersistence(builder.Configuration);
            builder.Services.RegisterIdentity();
            builder.Services.RegisterJwt(builder.Configuration);
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddGoogleAuthentication(builder.Configuration);



            builder.Services.AddLogging(options =>
            {
                options.AddLog4Net();
            });

            //builder.Services.AddControllers();

            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value!.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)      // !!! Use only in development to preview senstiive error message 
                                                          /////////
                                                          //.Select(x => MapErrorMessage(x.ErrorMessage))   // Use this in production to prevent senstiive error message exposure
                        .ToList();

                    var result = new ValidationResultModel
                    {
                        Status = false,
                        Message = "Some Errors were found ",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(result);
                };

                // Method to map detailed error messages to user-friendly messages
                string MapErrorMessage(string errorMessage)
                {
                    if (errorMessage.Contains("The JSON value could not be converted"))
                        return "Invalid data format. Please check your input.";

                    else if (errorMessage.Contains("The request field is required"))
                        return "Some required fields are missing.";

                    return "An error occurred. Please check your input.";
                }
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(swagger =>
            {
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \\r\\n\\r\\n Enter 'Bearer' [space] and then your token in the text input below.\\r\\n\\r\\nExample: \\\"Bearer 12345abcdef\\\"\"",
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference{
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
            /*if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }*/

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "RecipeDorm API v1");
                x.RoutePrefix = string.Empty;
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("MyCorsPolicy");

            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();
    
            app.MapControllers();

            app.Run();
        }
    }
}
