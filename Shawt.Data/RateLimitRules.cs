namespace Shawt.Data
{
    public partial class RateLimitRules
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string Endpoint { get; set; }
        public string Period { get; set; }
        public int Limit { get; set; }
    }
}
