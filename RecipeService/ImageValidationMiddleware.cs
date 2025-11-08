using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.API.Middleware
{
    public class ImageValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ImageValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Եթե հարցումը ունի ֆայլ upload
            if (context.Request.HasFormContentType && context.Request.Form.Files.Any())
            {
                foreach (var file in context.Request.Form.Files)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    // Եթե ֆորմատը սխալ է
                    if (!_allowedExtensions.Contains(extension))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(
                            $"❌ Only JPG, JPEG, and PNG formats are allowed. File '{file.FileName}' is not supported."
                        );
                        return;
                    }

                    // Եթե չափը շատ մեծ է (օր.՝ 5MB)
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(
                            $"❌ File '{file.FileName}' exceeds the 5MB limit."
                        );
                        return;
                    }
                }
            }

            // Եթե ամեն ինչ նորմալ է → փոխանցում ենք հաջորդ middleware-ին
            await _next(context);
        }
    }

    // Extension method՝ հեշտ միացնելու համար Program.cs-ում
    public static class ImageValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseImageValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageValidationMiddleware>();
        }
    }
}
