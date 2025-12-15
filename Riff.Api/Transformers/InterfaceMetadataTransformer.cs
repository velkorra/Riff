using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

namespace Riff.Api.Transformers;

public sealed class InterfaceMetadataTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var actionDescriptor = context.Description.ActionDescriptor as ControllerActionDescriptor;

        if (actionDescriptor == null) return Task.CompletedTask;

        var controllerType = actionDescriptor.ControllerTypeInfo;
        var actionMethod = actionDescriptor.MethodInfo;

        var interfaces = controllerType.GetInterfaces();

        foreach (var interfaceType in interfaces)
        {
            var interfaceMethod = interfaceType.GetMethod(
                actionMethod.Name,
                actionMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            if (interfaceMethod != null)
            {
                ApplyInterfaceAttributes(operation, interfaceMethod);
                break;
            }
        }

        return Task.CompletedTask;
    }

    private void ApplyInterfaceAttributes(OpenApiOperation operation, MethodInfo interfaceMethod)
    {
        var summaryAttr = interfaceMethod.GetCustomAttribute<EndpointSummaryAttribute>();
        if (summaryAttr != null && string.IsNullOrEmpty(operation.Summary))
        {
            operation.Summary = summaryAttr.Summary;
        }

        var descAttr = interfaceMethod.GetCustomAttribute<EndpointDescriptionAttribute>();
        if (descAttr != null && string.IsNullOrEmpty(operation.Description))
        {
            operation.Description = descAttr.Description;
        }

        var responseAttrs = interfaceMethod.GetCustomAttributes<ProducesResponseTypeAttribute>();
        foreach (var attr in responseAttrs)
        {
            var statusCode = attr.StatusCode.ToString();
            operation.Responses ??= new OpenApiResponses();
            if (!operation.Responses.ContainsKey(statusCode))
            {
                operation.Responses.Add(statusCode, new OpenApiResponse
                {
                    Description = ((System.Net.HttpStatusCode)attr.StatusCode).ToString()
                });
            }
        }
    }
}