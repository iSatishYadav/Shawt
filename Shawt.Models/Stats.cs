using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shawt.Models;


public partial class Stats
{
    [JsonPropertyName("clicks")]
    public long Clicks { get; set; }

    [JsonPropertyName("log")]
    public IEnumerable<Log> Log { get; set; }
}

public partial class Log
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("ip")]
    public string Ip { get; set; }
}

public partial class Stats
{
    public static Stats FromJson(string json) => string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Stats>(json);
    public static string ToJson(Stats self) => self == null ? null : JsonSerializer.Serialize(self);
}

