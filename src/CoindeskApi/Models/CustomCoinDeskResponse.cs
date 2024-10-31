using System;

namespace CoindeskApi.Models;

public class CustomCoinDeskResponse
{
    public DateTimeOffset? UpdatedISO { get; set; }
    public Dictionary<string, CustomCurrencyInfo>? Bpi { get; set; }

}

public class CustomCurrencyInfo
{
    public required string Code { get; set; }
    public string? Name { get; set; }
    public string? Rate { get; set; }
}
