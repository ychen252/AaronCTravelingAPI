using AaCTraveling.API.Database;
using AaCTraveling.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AaCTraveling.API.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AaCTraveling.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.ClaimsIdentity.UserIdClaimType = "Id";
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretKey = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters
                   = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidIssuer = Configuration["Authentication:Issuer"],

                       ValidateAudience = true,
                       ValidAudience = Configuration["Authentication:Audience"],

                       ValidateLifetime = true,

                       IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                   };
                });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services
                .AddControllers(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true;
                })
                //newtonSoft
                .AddNewtonsoftJson(setuoAction =>
                {
                    setuoAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                //xml support
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = (actionContext) =>
                    {
                        var problemDetail = new ValidationProblemDetails(actionContext.ModelState)
                        {
                            Type = "model-validation",
                            Title = "Model Validation Failed",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "See details",
                            Instance = actionContext.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("traceId", actionContext.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });
            //custom services
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            //db service
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(Configuration["DbContext:MySqlConnectionString"], ServerVersion.AutoDetect(Configuration["DbContext:MySqlConnectionString"]));
            });

            //scan for profile file
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpClient();

            //add vendor media type
            services.Configure<MvcOptions>(config =>
            {
                var outputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (outputFormatter != null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.aac.hateoas+json");
                }
            });
            //cors
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context => {
                //    await context.Response.WriteAsync("Hello FROM TEST!");
                //});
                //endpoints.MapGet("/test", async context => {
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
            });
        }
    }
}
