using System;
using CoindeskApi.DTOs;
using CoindeskApi.Models;

namespace CoindeskApi.Data;

public interface ICurrenciesRepository
{
    Task<IEnumerable<Currency>> GetCurrencies();
    Task<Currency> GetCurrency(int id);
    Task<CurrencyDTO> GetCurrencyByCode(string code);
    void AddCurrency(Currency currency);
    void RemoveCurrency(Currency currency);
    Task<bool> SaveChangeAsync();




}
