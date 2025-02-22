using System;
using System.Collections.Generic;

namespace Shawt.Data
{
    public partial class Link
    {
        public Link()
        {
            Log = new HashSet<Log>();
        }

        public int Id { get; set; }
        public Guid LinkId { get; set; }
        public string OriginalLink { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Stats { get; set; }

        public virtual ICollection<Log> Log { get; set; }
    }
}
