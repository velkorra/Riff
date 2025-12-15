namespace Riff.Api.Contracts.Exceptions;

public class ResourceNotFoundException : Exception
{
    public string ResourceName { get; }
    public object ResourceId { get; }

    public ResourceNotFoundException(string resourceName, object resourceId)
        : base($"Resource '{resourceName}' with ID '{resourceId}' was not found.")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }
}