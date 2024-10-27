using System;
using AutoMapper;
using CoindeskApi.Data;
using CoindeskApi.DTOs;
using CoindeskApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CurrenciesController : ControllerBase
{
    private readonly CoinDbContext _context;
    private readonly IMapper _mapper;

    public CurrenciesController(CoinDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all currencies.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
    {
        return await _context.Currencies.ToListAsync();
    }


    /// <summary>
    /// Get currency by Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Currency>> GetCurrency(int id)
    {
        var currency = await _context.Currencies.FindAsync(id);

        if (currency == null)
        {
            return NotFound();
        }

        return currency;
    }

    /// <summary>
    /// Create a new currency record.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Currencies
    ///     {
    ///        "Code": "JPY",
    ///        "Name": "日圓"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Currency>> PostCurrency(CreateCurrencyDTO currencyDto)
    {
        var currency = _mapper.Map<Currency>(currencyDto);
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync();
        var newCurrency = _mapper.Map<CurrencyDTO>(currency);
        return CreatedAtAction(nameof(GetCurrency), new { id = newCurrency.Id }, newCurrency);
    }

    /// <summary>
    /// Update the Name of the currency by Id.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCurrency(int id, Currency currency)
    {
        if (id != currency.Id)
        {
            return BadRequest();
        }

        _context.Entry(currency).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete a curency by Id.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCurrency(int id)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency == null)
        {
            return NotFound();
        }

        _context.Currencies.Remove(currency);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
