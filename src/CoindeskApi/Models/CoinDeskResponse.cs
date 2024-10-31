using System;
using Newtonsoft.Json;

namespace CoindeskApi.Models;

public class CoinDeskResponse
{
    [JsonProperty("time")]
    public TimeInfo? Time { get; set; }
    public Dictionary<string, CurrencyInfo>? Bpi { get; set; }
}

public class TimeInfo
{
    [JsonProperty("updated")]
    public string? Updated { get; set; }
    [JsonProperty("updatedISO")]
    public DateTimeOffset UpdatedISO { get; set; }
    [JsonProperty("updateduk")]
    public string? Updateduk { get; set; }
}

public class CurrencyInfo
{
    public required string Code { get; set; }
    public required string Rate { get; set; }
    public string? Description { get; set; }
}