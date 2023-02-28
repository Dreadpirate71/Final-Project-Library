using LibraryAPI.Daos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI
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
            services.AddSingleton<DapperContext>();
            services.AddControllers();
            services.AddScoped<BookDao>();
            services.AddScoped<IStaffDao, StaffDao>();
            services.AddScoped<IBookDao, BookDao>();
            services.AddControllers();
            services.AddScoped<PatronDao>();
            services.AddScoped<IPatronDao, PatronDao>();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LibraryApi",
                Description = "A CRUD Api for managing a library.",
                Version = "v1"
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraryApi V1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
