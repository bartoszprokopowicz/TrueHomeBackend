using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;

namespace ResourceServer.Helpers
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory
                .CreateLogger<RequestResponseLoggingMiddleware>();
        }
    
        public async Task Invoke(HttpContext context)
        {
            _logger.LogDebug(await FormatRequest(context.Request));

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                _logger.LogDebug(await FormatResponse(context.Response));
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    
        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableRewind();
            var body = request.Body;

            var buffer = new byte[request.ContentLength < 1000 ?
                Convert.ToInt32(request.ContentLength) : 1000];

            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            request.Body = body;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text;
            if (response.ContentLength < 1000)
                text = await new StreamReader(response.Body).ReadToEndAsync();
            else
            {
                var buffer = new char[1000];
                await new StreamReader(response.Body).ReadAsync(buffer, 0, buffer.Length);
                text = new string(buffer);
            }

            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Response {text}";
        }
    }
}