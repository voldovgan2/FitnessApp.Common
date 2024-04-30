using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace FitnessApp.Common.Middleware
{
    public abstract class AbstractRequestResponseLoggingMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 110100480;

            var stringBuilder = new StringBuilder(Environment.NewLine);
            await AppendRequestData(context.Request, stringBuilder);

            using var copyBodyStream = new MemoryStream();
            context.Response.Body = copyBodyStream;

            await next(context);
            await AppendResponseBody(context.Response.StatusCode, copyBodyStream, context.Request.Path, stringBuilder);

            context.Response.Body = copyBodyStream;
            context.Features.Get<ILogger>().Information(stringBuilder.ToString());
        }

        private async Task AppendRequestData(HttpRequest request, StringBuilder stringBuilder)
        {
            stringBuilder.Append($"REQUEST HttpMethod: {request.Method}, Path: {request.Path}, Query: {request.QueryString.Value}");
            stringBuilder.Append($"REQUEST Headers:");
            foreach (var header in request.Headers)
                stringBuilder.Append($"{Environment.NewLine} {header.Key} : {header.Value}");

            if (!(request.Headers.ContainsKey("content-type") && request.Headers["content-type"][0].StartsWith("multipart/form-data")))
            {
                request.EnableBuffering();
                var bodyReader = new StreamReader(request.Body);
                var bodyText = await bodyReader.ReadToEndAsync();
                request.Body.Position = 0;
                if (!string.IsNullOrWhiteSpace(bodyText))
                    stringBuilder.Append(Environment.NewLine + $" Body : {ObfuscateBodyText(RequestDirection.In, bodyText, request.Path)}");
            }
        }

        private async Task AppendResponseBody(int statusCode, Stream stream, string path, StringBuilder stringBuilder)
        {
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var responseBodyText = ObfuscateBodyText(RequestDirection.Out, await reader.ReadToEndAsync(), path);
            stream.Position = 0;
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append($"RESPONSE HTTP StatusCode: {statusCode}, BODY: {responseBodyText}");
        }

        protected abstract string ObfuscateBodyText(RequestDirection requestDirection, string bodyText, string path);
    }
}
