using System;
using CoindeskApi.Models;
using Newtonsoft.Json;

namespace CoindeskApi.Services;

public class CoinDeskService : ICoinDeskService
{
    private readonly HttpClient _httpClient;

    public CoinDeskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CoinDeskResponse?> GetCoinDeskDataAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: Received status code {response.StatusCode}");
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CoinDeskResponse>(content);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return null;

    }
}
