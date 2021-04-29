using AutoMapper;
using CQRSMeetup.Core.Mapper;
using CQRSMeetup.Core.Models;
using CQRSMeetup.Core.Redis;
using CQRSMeetup.Core.Swagger;
using CQRSMeetup.ReadApi.Features.Stock.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace CQRSMeetup.ReadApi
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
            services.Configure<RedisConfigModel>(Configuration.GetSection("RedisConfig"));
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            #region Services Register
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRedisService, RedisService>();
            #endregion

            #region AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile("CQRSMeetup.ReadApi"));
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
                   .WithMethods("GET")
                   .AllowAnyHeader()
                   .SetPreflightMaxAge(System.TimeSpan.FromMinutes(60))
                   .Build();
                });
            });
            #endregion
            services.AddControllers();

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
