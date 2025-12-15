namespace Riff.Api.Contracts.Dto;

/// <summary>
/// Represents a HATEOAS link according to HAL standards.
/// </summary>
/// <param name="Href">The target URL of the link.</param>
/// <param name="Rel">The relation of the link to the resource.</param>
/// <param name="Method">The HTTP method to use for the request.</param>
public record LinkDto(string Href, string Rel, string Method);