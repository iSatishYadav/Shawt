using System.ComponentModel.DataAnnotations;

namespace Shawt.Models
{
    public class LongUrl
    {
        [Url]
        public string Url { get; set; }
    }
}
