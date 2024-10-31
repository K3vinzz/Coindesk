using System;
using CoindeskApi.Models;

namespace CoindeskApi.Services;

public interface ICoinDeskService
{
    Task<CoinDeskResponse?> GetCoinDeskDataAsync();
}
