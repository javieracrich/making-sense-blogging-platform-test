using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Api
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            var requiredScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Distinct();

            if (requiredScopes.Any())
            {
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>> {{"Bearer", new string[] { }}}
                };
            }
        }
    }

    public class SetRightContentTypes : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Produces?.Any() == true)
            {
                operation.Produces.Clear();
                operation.Produces.Add("application/json");
            }

            if (operation.Consumes?.Any() == true)
            {
                operation.Consumes.Clear();
                operation.Produces.Add("application/json");
            }

            if (operation.OperationId == "UserByUserIdImagePost" ||
                operation.OperationId == "SchoolBySchoolIdImagePost" ||
                operation.OperationId == "CardBulk-insertPost")
            {

                var objFile = operation.Parameters.First(x => x.Name == "file");

                operation.Parameters.Remove(objFile);

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "file",
                    In = "formData",
                    Description = "Upload File",
                    Required = true,
                    Type = "file"
                });

                operation.Consumes.Clear();
                operation.Consumes.Add("application/form-data");
            }
        }
    }
}