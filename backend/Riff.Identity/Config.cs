using Duende.IdentityServer.Models;

namespace Riff.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
            new IdentityResources.Profile() 
    ];

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("riff_api", "Riff API")
            {
                Scopes = { "riff_api" }
            }
        };
    
    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("riff_api", "Full Access to Riff API")
    ];

    public static IEnumerable<Client> Clients =>
    [
            new Client
            {
                ClientId = "riff_frontend",
                ClientName = "Riff Web Client",

                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,

                AllowedCorsOrigins = { "https://riff.local.oshideck.app" }, 
                RedirectUris = { "https://riff.local.oshideck.app/callback" },
                PostLogoutRedirectUris = { "https://riff.local.oshideck.app" },

                AllowedScopes = { "openid", "profile", "riff_api" },
                
                AccessTokenLifetime = 3600 
            },
            
            new Client
            {
                ClientId = "riff_swagger",
                ClientName = "Swagger UI",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                AllowedCorsOrigins = { "https://riff.local.oshideck.app" }, 
                RedirectUris = { "https://riff.local.oshideck.app/callback" },
                PostLogoutRedirectUris = { "https://riff.local.oshideck.app" },
                AllowedScopes = { "openid", "profile", "riff_api" }
            }
    ];
}