using System;

namespace Shawt.Data
{
    public partial class RateLimitCounters
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Count { get; set; }
    }
}
