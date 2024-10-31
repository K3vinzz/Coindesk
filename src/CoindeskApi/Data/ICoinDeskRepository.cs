using System;
using CoindeskApi.Models;

namespace CoindeskApi.Data;

public interface ICoinDeskRepository
{
    Task<CoinDeskResponse?> GetCoinDeskDataAsync();
    Task<List<Currency>> GetCurrenciesAsync();
}
