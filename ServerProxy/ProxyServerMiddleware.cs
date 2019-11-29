using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerProxy
{
     public class ProxyServerMiddleware
     {
          private readonly RequestDelegate _next;

          public ProxyServerMiddleware(RequestDelegate next)
          {
               _next = next;
          }

          public async Task Invoke(HttpContext context)
          {
               var endRequest = false;
               if (context.Request.Path.Value.Equals("/proxy", StringComparison.OrdinalIgnoreCase))
               {
                    const string key = "url";
                    if (context.Request.Query.ContainsKey(key))
                    {
                         var url = context.Request.Query[key][0];
                         await StreamAsync(context, url);
                         endRequest = true;
                    }
               }
               if (!endRequest)
               {
                    await _next(context);
               }
          }

          private static async Task StreamAsync(HttpContext context, string url)
          {
               var httpClientHandler = new HttpClientHandler
               {
                    AllowAutoRedirect = false
               };
               var webRequest = new HttpClient(httpClientHandler);

               var buffer = new byte[4 * 1024];
               var localResponse = context.Response;
               try
               {
                    using (var remoteStream = await webRequest.GetStreamAsync(url))
                    {
                         var bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                         localResponse.Clear();
                         localResponse.ContentType = "application/octet-stream";
                         var fileName = Path.GetFileName(url);
                         localResponse.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

                         if (remoteStream.Length != -1)
                              localResponse.ContentLength = remoteStream.Length;

                         while (bytesRead > 0) // && localResponse.IsClientConnected)
                         {
                              await localResponse.Body.WriteAsync(buffer, 0, bytesRead);
                              bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                         }
                    }
               }
               catch (Exception e)
               {
                    // Do some logging here
               }
          }

          public static async Task<string> FormatResponse(HttpContext context)
          {
               var response = context.Response;
               //read response stream from beginning
               response.Body.Seek(0, SeekOrigin.Begin);
               //cpory to string
               string text = await new StreamReader(response.Body).ReadToEndAsync();
               Console.Write(text);
               response.Body.Seek(0, SeekOrigin.Begin);
               return text;
          }
     }
}
