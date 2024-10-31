using System;
using AutoMapper;
using CoindeskApi.DTOs;
using CoindeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Data;

public class CurrenciesRepository : ICurrenciesRepository
{
    private readonly CoinDbContext _context;
    private readonly IMapper _mapper;

    public CurrenciesRepository(CoinDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Currency>> GetCurrencies()
    {
        return await _context.Currencies.ToListAsync();
    }

    public async Task<CurrencyDTO> GetCurrencyById(int id)
    {
        var currency =  await _context.Currencies.FindAsync(id);
        return _mapper.Map<CurrencyDTO>(currency);
    }

    public async Task<Currency?> GetCurrencyModelById(int id)
    {
        return await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<CurrencyDTO> GetCurrencyByCode(string code)
    {
        var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.Code == code);
        return _mapper.Map<CurrencyDTO>(currency);
    }

    public async Task<Currency?> GetCurrencyModelByCode(string code)
    {
        return await _context.Currencies.FirstOrDefaultAsync(c => c.Code == code);
    }

    public void AddCurrency(Currency currency)
    {
        _context.Currencies.Add(currency);
    }

    public void RemoveCurrency(Currency currency)
    {
        _context.Currencies.Remove(currency);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
