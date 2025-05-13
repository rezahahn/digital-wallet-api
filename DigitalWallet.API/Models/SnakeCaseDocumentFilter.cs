using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DigitalWallet.API.Models
{
    public class SnakeCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                paths.Add(ToSnakeCase(path.Key), path.Value);
            }
            swaggerDoc.Paths = paths;
        }

        private string ToSnakeCase(string path) => path.ToLower();
    }
}
