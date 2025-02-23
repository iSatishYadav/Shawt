using System;

namespace Shawt.Models;

public class LogDto
{
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime? Timestamp { get; set; }
    public string Browser { get; set; }
    public string Os { get; set; }
    public string Device { get; set; }
}