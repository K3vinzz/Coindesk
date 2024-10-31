using System;
using CoindeskApi.Models;
using CoindeskApi.Services;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Data;

public class CoinDeskRepository : ICoinDeskRepository
{
    private readonly ICoinDeskService _service;
    private readonly CoinDbContext _context;

    public CoinDeskRepository(ICoinDeskService service, CoinDbContext context)
    {
        _service = service;
        _context = context;
    }

    public Task<CoinDeskResponse?> GetCoinDeskDataAsync()
    {
        return _service.GetCoinDeskDataAsync();
    }

    public Task<List<Currency>> GetCurrenciesAsync()
    {
        return _context.Currencies.ToListAsync();
    }
}
