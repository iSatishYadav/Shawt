using System;

namespace Shawt.Models;

public class LinkDto
{
    public Guid Id { get; set; }
    public string OriginalLink { get; set; }
    public string ShortLink { get; set; }
    public int? Clicks { get; set; }
    public DateTime CreatedOn { get; set; }
}
