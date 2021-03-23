using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using spendingAPI.Data;
using spendingAPI.Exceptions;
using spendingAPI.Repositories;
using spendingAPI.Services;

namespace spendingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<SpendingContext>(options =>
            //{
            //    //var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //    string connStr;

            //    // Use connection string provided at runtime by Heroku.
            //    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            //    // Parse connection URL to connection string for Npgsql
            //    connUrl = connUrl.Replace("postgres://", string.Empty);
            //    var pgUserPass = connUrl.Split("@")[0];
            //    var pgHostPortDb = connUrl.Split("@")[1];
            //    var pgHostPort = pgHostPortDb.Split("/")[0];
            //    var pgDb = pgHostPortDb.Split("/")[1];
            //    var pgUser = pgUserPass.Split(":")[0];
            //    var pgPass = pgUserPass.Split(":")[1];
            //    var pgHost = pgHostPort.Split(":")[0];
            //    var pgPort = pgHostPort.Split(":")[1];

            //    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};sslmode=Prefer;Trust Server Certificate=true";

            //    options.UseNpgsql(connStr);

            //    // Depending on if in development or production, use either Heroku-provided
            //    // connection string, or development connection string from env var.
            //    //if (env == "Development")
            //    //{
            //    //    // Use connection string from file.
            //    //    connStr = config.GetConnectionString("DefaultConnection");
            //    //}
            //    //else
            //    //{
            //    //    // Use connection string provided at runtime by Heroku.
            //    //    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            //    //    // Parse connection URL to connection string for Npgsql
            //    //    connUrl = connUrl.Replace("postgres://", string.Empty);
            //    //    var pgUserPass = connUrl.Split("@")[0];
            //    //    var pgHostPortDb = connUrl.Split("@")[1];
            //    //    var pgHostPort = pgHostPortDb.Split("/")[0];
            //    //    var pgDb = pgHostPortDb.Split("/")[1];
            //    //    var pgUser = pgUserPass.Split(":")[0];
            //    //    var pgPass = pgUserPass.Split(":")[1];
            //    //    var pgHost = pgHostPort.Split(":")[0];
            //    //    var pgPort = pgHostPort.Split(":")[1];

            //    //    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}";
            //    //}
            //});

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<SpendingContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddScoped<IEntryService, EntryService>();
            services.AddScoped<IJWTService, JWTService>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
                .AddJwtBearer("JwtBearer", jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TokenKey"))),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("jasdvjasdpdvjsadjvpsamdvpsofapojasfsdpjcasdmovpadsmvpasdvmsa")),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    var errorResponse = DetermineErrorResponse(exception);

                    var jsonResponse = JsonConvert.SerializeObject(errorResponse);
                    var content = Encoding.UTF8.GetBytes(jsonResponse);

                    context.Response.Headers.Add("Content-Type", "application/json");
                    context.Response.StatusCode = errorResponse.StatusCode;
                    context.Response.Body.WriteAsync(content, 0, content.Length);

                    return Task.CompletedTask;
                }
            });

            app.UseCors(options => {
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private ErrorResponse DetermineErrorResponse(IExceptionHandlerFeature exception)
        {
            var response = new ErrorResponse();

            switch (exception.Error)
            {
                case ArgumentException _:
                    response.StatusCode = 400;
                    response.Message = exception.Error.Message;
                    break;
                case NotFoundException _:
                    response.StatusCode = 404;
                    response.Message = exception.Error.Message;
                    break;
                default:
                    response.StatusCode = 500;
                    response.Message = "Internal server error";
                    break;
            }

            return response;
        }

        private class ErrorResponse
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }
    }
}
