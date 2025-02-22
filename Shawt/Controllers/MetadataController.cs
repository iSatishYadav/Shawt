using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shawt.Models;

namespace Shawt.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public MetadataController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _clientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: Metadata
        [HttpGet]
        public async Task<OpenIdClientSettings> Get()
        {
            using var client = _clientFactory.CreateClient();
            var result = await client.GetAsync(_configuration["Authorization:KeysUrl"]);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStreamAsync();
            var keys1 = await JsonSerializer.DeserializeAsync<Jwks>(response);
            //TODO: Get this value from well-known endpoint
            return new OpenIdClientSettings
            {
                Authority = _configuration["Authorization:Authority"],
                Client_id = _configuration["Authorization:ClientId"],
                FilterProtocolClaims = true,
                LoadUserInfo = false,
                Post_logout_redirect_uri = Url.Action("", null, null, Request.Scheme),
                Redirect_uri = Url.Action("", "auth-callback", null, _configuration["Authorization:RedirectionUri:Request:Scheme"]),
                Response_type = _configuration["Authorization:ResponseType"],
                Scope = _configuration["Authorization:Scope"],
                Metadata = new AuthorizationMetadata
                {
                    //Jwks_uri = Url.Action(@"keys", "Metadata"),
                    AuthorizationEndpoint = _configuration["Authorization:AuthorizationEndpoint"],
                    Issuer = _configuration["Authorization:Issuer"],
                    TokenEndpoint = _configuration["Authorization:TokenEndpoint"],
                    UserinfoEndpoint = _configuration["Authorization:UserInfoEndpoint"]
                },
                SigningKeys = keys1.Keys
            };
        }
    }
}
