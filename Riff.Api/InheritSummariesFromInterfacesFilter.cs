using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Riff.Api;

public class InheritAttributesFromInterfacesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerMethodInfo = context.MethodInfo;
        var implementedInterfaces = controllerMethodInfo.DeclaringType!.GetInterfaces();

        foreach (var interfaceType in implementedInterfaces)
        {
            var interfaceMethodInfo = interfaceType.GetMethod(
                controllerMethodInfo.Name,
                controllerMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            if (interfaceMethodInfo == null) continue;

            if (string.IsNullOrEmpty(operation.Summary) && string.IsNullOrEmpty(operation.Description))
            {
                var operationAttribute = interfaceMethodInfo.GetCustomAttribute<SwaggerOperationAttribute>();
                if (operationAttribute != null)
                {
                    operation.Summary = operationAttribute.Summary;
                    operation.Description = operationAttribute.Description;
                }
            }

            var responseAttributes = interfaceMethodInfo.GetCustomAttributes<SwaggerResponseAttribute>().ToList();
            foreach (var attr in responseAttributes)
            {
                var statusCode = attr.StatusCode.ToString();

                if (operation.Responses?.ContainsKey(statusCode) ?? false) continue;

                var response = new OpenApiResponse
                {
                    Description = attr.Description
                };

                var schema = context.SchemaGenerator.GenerateSchema(attr.Type, context.SchemaRepository);
                response.Content ??= new Dictionary<string, OpenApiMediaType>();
                response.Content.Add("application/json", new OpenApiMediaType { Schema = schema });

                operation.Responses ??= new OpenApiResponses();
                operation.Responses.Add(statusCode, response);

                context.SchemaRepository.TryLookupByType(attr.Type, out _);
            }

            if (responseAttributes.Any() || operation.Summary != null)
            {
                break;
            }
        }
    }
}