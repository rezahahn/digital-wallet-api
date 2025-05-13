using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DigitalWallet.API.Models
{
    public class SnakeCaseSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null) return;

            var newProps = new Dictionary<string, OpenApiSchema>();
            foreach (var prop in schema.Properties)
            {
                newProps[ToSnakeCase(prop.Key)] = prop.Value;
            }
            schema.Properties = newProps;
        }

        private string ToSnakeCase(string name) =>
            string.Concat(name.Select((c, i) =>
                i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLower();
    }
}
