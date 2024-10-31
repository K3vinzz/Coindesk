using System;
using CoindeskApi.DTOs;
using CoindeskApi.Models;

namespace CoindeskApi.Data;

public interface ICurrenciesRepository
{
    Task<List<Currency>> GetCurrencies();
    Task<CurrencyDTO> GetCurrencyById(int id);
    Task<Currency?> GetCurrencyModelById(int id);
    Task<CurrencyDTO> GetCurrencyByCode(string code);
    Task<Currency?> GetCurrencyModelByCode(string code);
    void AddCurrency(Currency currency);
    void RemoveCurrency(Currency currency);
    Task<int> SaveChangesAsync();
}
