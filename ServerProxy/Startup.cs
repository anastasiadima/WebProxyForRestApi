using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ServerProxy
{
     public class Startup
     {
          // This method gets called by the runtime. Use this method to add services to the container.
          // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
          public void ConfigureServices(IServiceCollection services)
          {
               services.AddDistributedRedisCache(options =>
               {
                    options.Configuration = "localhost:6379";
               });
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IHostingEnvironment env)
          {
               if (env.IsDevelopment())
               {
                    app.UseDeveloperExceptionPage();
               }

               app.Map("", config => { config.Use(async (ctx, next) =>
                    {
                         await ctx.Response.WriteAsync("hi");
               }); }
                );
               
               app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/api/Books"), builder => builder.RunProxy(new ProxyOptions
               {
                    Scheme = "https",
                    Host = "localhost",
                    Port = "5001"
               }));
          }
     }
}
