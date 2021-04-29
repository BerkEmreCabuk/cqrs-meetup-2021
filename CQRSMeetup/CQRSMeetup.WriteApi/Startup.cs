using AutoMapper;
using CQRSMeetup.Core.Mapper;
using CQRSMeetup.Core.Models;
using CQRSMeetup.Core.RabbitMq;
using CQRSMeetup.Core.Swagger;
using CQRSMeetup.WriteApi.Features.Stock.Validators;
using CQRSMeetup.WriteApi.Infrastructures.Database;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace CQRSMeetup.WriteApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RabbitMqConfigModel>(Configuration.GetSection("RabbitMqConfig"));

            services.AddDbContext<CQRSMeetupDbContext>(options => options.UseInMemoryDatabase(databaseName: "CQRSMeetupDb"));
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            #region Services Register
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddScoped<IRepository, Repository>();
            #endregion

            #region AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile("CQRSMeetup.WriteApi"));
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region CORS Enable
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                   .WithMethods("POST", "PUT", "PATCH", "DELETE")
                   .AllowAnyHeader()
                   .SetPreflightMaxAge(System.TimeSpan.FromMinutes(60))
                   .Build();
                });
            });
            #endregion

            #region Fluent Validation
            services.AddControllers()
                .AddJsonOptions(setupAction => new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                })
                .AddFluentValidation(validationConfig => validationConfig.RegisterValidatorsFromAssembly(Assembly.GetAssembly(typeof(ChangeStockStatusValidator))));
            #endregion

            #region ApiVersioning & Swagger
            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    //header version örneği
                    //options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
            services.AddSwaggerGen();

            services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
