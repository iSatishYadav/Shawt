using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shawt.Models;

namespace Shawt.Controllers;

[Route("[controller]")]
[ApiController]
public class MetadataController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ControllerBase
{

    // GET: Metadata
    [HttpGet]
    public async Task<OpenIdClientSettings> Get()
    {
        using var client = httpClientFactory.CreateClient();
        var result = await client.GetAsync(configuration["Authorization:KeysUrl"]);
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadAsStreamAsync();
        var keys1 = await JsonSerializer.DeserializeAsync<Jwks>(response);
        //TODO: Get this value from well-known endpoint
        return new OpenIdClientSettings
        {
            Authority = configuration["Authorization:Authority"],
            Client_id = configuration["Authorization:ClientId"],
            FilterProtocolClaims = true,
            LoadUserInfo = false,
            Post_logout_redirect_uri = Url.Action("", null, null, Request.Scheme),
            Redirect_uri = Url.Action("", "auth-callback", null, configuration["Authorization:RedirectionUri:Request:Scheme"]),
            Response_type = configuration["Authorization:ResponseType"],
            Scope = configuration["Authorization:Scope"],
            Metadata = new AuthorizationMetadata
            {
                //Jwks_uri = Url.Action(@"keys", "Metadata"),
                AuthorizationEndpoint = configuration["Authorization:AuthorizationEndpoint"],
                Issuer = configuration["Authorization:Issuer"],
                TokenEndpoint = configuration["Authorization:TokenEndpoint"],
                UserinfoEndpoint = configuration["Authorization:UserInfoEndpoint"]
            },
            SigningKeys = keys1.Keys
        };
    }
}
