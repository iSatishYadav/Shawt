using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shawt.Models;

public class SigningKey
{
    [JsonProperty("kty")]
    [JsonPropertyName("kty")]
    public string Kty { get; set; }
    [JsonProperty("use")]

    [JsonPropertyName("use")]
    public string Use { get; set; }
    [JsonProperty("alg")]
    [JsonPropertyName("alg")]
    public string Alg { get; set; }
    [JsonProperty("kid")]
    [JsonPropertyName("kid")]
    public string Kid { get; set; }
    [JsonProperty("x5t")]
    [JsonPropertyName("x5t")]
    public string X5t { get; set; }
    [JsonProperty("n")]
    [JsonPropertyName("n")]
    public string N { get; set; }
    [JsonProperty("e")]
    [JsonPropertyName("e")]
    public string E { get; set; }
    [JsonProperty("x5c")]
    [JsonPropertyName("x5c")]
    public List<string> X5c { get; }
}

public class AuthorizationMetadata
{
    [JsonProperty("issuer")]
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; }
    [JsonProperty("authorization_endpoint")]
    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; }
    [JsonProperty("token_endpoint")]
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; }
    [JsonProperty("userinfo_endpoint")]
    [JsonPropertyName("userinfo_endpoint")]
    public string UserinfoEndpoint { get; set; }
    [JsonProperty("jwks_uri")]
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; }
}

public class OpenIdClientSettings
{
    [JsonProperty("authority")]
    [JsonPropertyName("authority")]
    public string Authority { get; set; }
    [JsonProperty("client_id")]
    [JsonPropertyName("client_id")]
    public string Client_id { get; set; }
    [JsonProperty("redirect_uri")]
    [JsonPropertyName("redirect_uri")]
    public string Redirect_uri { get; set; }
    [JsonProperty("post_logout_redirect_uri")]
    [JsonPropertyName("post_logout_redirect_uri")]
    public string Post_logout_redirect_uri { get; set; }
    [JsonProperty("response_type")]
    [JsonPropertyName("response_type")]
    public string Response_type { get; set; }
    [JsonProperty("scope")]
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
    [JsonProperty("filterProtocolClaims")]
    [JsonPropertyName("filterProtocolClaims")]
    public bool FilterProtocolClaims { get; set; }
    [JsonProperty("loadUserInfo")]
    [JsonPropertyName("loadUserInfo")]
    public bool LoadUserInfo { get; set; }
    [JsonProperty("signingKeys")]
    [JsonPropertyName("signingKeys")]
    public IEnumerable<SigningKey> SigningKeys { get; set; }
    [JsonProperty("metadata")]
    [JsonPropertyName("metadata")]
    public AuthorizationMetadata Metadata { get; set; }
}
