using System;
using Newtonsoft.Json;

namespace CoindeskApi.Services;

public class CoinDeskService
{
    private readonly HttpClient _httpClient;

    public CoinDeskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CoinDeskResponse?> GetCoinDeskDataAsync()
    {
        var response = await _httpClient.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: Received status code {response.StatusCode}");
            return null;
        }
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CoinDeskResponse>(content)!;
    }

    public class CoinDeskResponse
    {
        [JsonProperty("time")]
        public TimeInfo? Time { get; set; }
        public Dictionary<string, CurrencyInfo>? Bpi { get; set; }
    }

    public class CurrencyInfo
    {
        public required string Code { get; set; }
        public required string Rate { get; set; }
        public string? Description { get; set; }
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
}
