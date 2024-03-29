﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Proxy.Models;
using Proxy.Service;

namespace Proxy
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
               services.Configure<BookstoreDatabaseSettings>(
                    Configuration.GetSection(nameof(BookstoreDatabaseSettings)));

               services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
                    sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);
               
               services.AddSingleton<BookService>();

               services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IHostingEnvironment env)
          {
               if (env.IsDevelopment())
               {
                    app.UseDeveloperExceptionPage();
               }
               else
               {
                    app.UseHsts();
               }
               app.UseMiddleware<ReverseProxyMiddleware>();
               app.UseHttpsRedirection();
               app.UseMvc();
          }
     }
}
