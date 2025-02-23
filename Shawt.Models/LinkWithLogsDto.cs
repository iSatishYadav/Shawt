using System;
using System.Collections.Generic;

namespace Shawt.Models;

public class LinkWithLogsDto
{

    public Guid LinkId { get; set; }
    public string OriginalLink { get; set; }
    public string ShortLink { get; set; }
    public DateTime CreatedOn { get; set; }
    public long Clicks { get; set; }

    public IEnumerable<LogDto> Logs { get; set; }

}
