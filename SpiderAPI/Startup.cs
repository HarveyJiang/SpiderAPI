using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SpiderAPI.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace SpiderAPI
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

            services.AddCors(options =>
            {
                options.AddPolicy("my_cors", builder => builder.WithOrigins("https://mysite.com"));
            });
            //services.Configure<StorageOptions>(Configuration.GetSection("AzureStorageConfig"));
            //services.Configure<SiteConfig>(Configuration.GetSection("SiteConfig"));
            services.AddSingleton(Configuration);

            //services.AddSingleton()
            //services.AddTransient<IProductRepository, ProductRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SPIDER API", Version = "v1", Description = "By Harvey" });
                c.DescribeStringEnumsInCamelCase();
                c.DescribeAllEnumsAsStrings();
            });
            services.AddTransient<IRepository<SpiderBasic>, Repository<SpiderBasic>>();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseMvc();
            app.UseCors("my_cors");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                c.DocumentTitle = "Spider API v1";
                c.DocExpansion(DocExpansion.None);
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

        }
    }

}
