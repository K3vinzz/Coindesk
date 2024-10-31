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
    private readonly ICurrenciesRepository _repo;
    private readonly IMapper _mapper;

    public CurrenciesController(ICurrenciesRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all currencies.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Currency>>> GetCurrencies()
    {
        return await _repo.GetCurrencies();
    }


    /// <summary>
    /// Get currency by Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CurrencyDTO>> GetCurrencyById(int id)
    {
        var currency = await _repo.GetCurrencyById(id);

        if (currency == null)
        {
            return NotFound();
        }

        return currency;
    }

    /// <summary>
    /// Get currency by Code.
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<CurrencyDTO>> GetCurrencyByCode(string code)
    {
        var currency = await _repo.GetCurrencyByCode(code);
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
    public async Task<ActionResult<CurrencyDTO>> AddCurrency(CreateCurrencyDTO currencyDto)
    {
        var currency = _mapper.Map<Currency>(currencyDto);
        _repo.AddCurrency(currency);
        var result = await _repo.SaveChangesAsync();
        var newCurrency = _mapper.Map<CurrencyDTO>(currency);
        if (result == 0) return BadRequest("Fail to add currency.");
        return CreatedAtAction(nameof(GetCurrencyById), new {id = newCurrency.Id}, newCurrency);
    }

    /// <summary>
    /// Update the Name of the currency by Id.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCurrencyById(int id, UpdateCurrencyDTO updateCurrencyDTO)
    {
        var currency = await _repo.GetCurrencyModelById(id);

        if (currency == null)
        {
            return NotFound();
        }
        else if (updateCurrencyDTO.Code == currency.Code && updateCurrencyDTO.Name == currency.Name)
        {
            return NoContent();
        }

        currency.Code = updateCurrencyDTO.Code ?? currency.Code;
        currency.Name = updateCurrencyDTO.Name ?? currency.Name;

        var result = await _repo.SaveChangesAsync();

        if (result > 0) return Ok();

        return BadRequest("Problem saving changes");
    }

    /// <summary>
    /// Update the currency by Code.
    /// </summary>
    [HttpPut("code/{code}")]
    public async Task<IActionResult> UpdateCurrencyByCode(string code, string name)
    {
        var currency = await _repo.GetCurrencyModelByCode(code);

        if (currency == null)
        {
            return NotFound();
        }
        else if (currency.Name == name)
        {
            return NoContent();
        }

        currency.Name = name;

        var result = await _repo.SaveChangesAsync();

        if (result > 0) return Ok();

        return BadRequest("Problem saving changes");
    }



    /// <summary>
    /// Delete a curency by Id.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveCurrencyById(int id)
    {
        var currency = await _repo.GetCurrencyModelById(id);

        if (currency == null)
        {
            return NotFound();
        }

        _repo.RemoveCurrency(currency);

        var result = await _repo.SaveChangesAsync();

        if (result > 0) return Ok();

        return BadRequest("Problem saving changes");
    }

    /// <summary>
    /// Delete a currency by Code.
    /// </summary>
    [HttpDelete("code/{code}")]
    public async Task<IActionResult> RemoveCurrencyByCode(string code)
    {
        var currency = await _repo.GetCurrencyModelByCode(code);

        if (currency == null)
        {
            return NotFound();
        }

        _repo.RemoveCurrency(currency);

        var result = await _repo.SaveChangesAsync();

        if (result > 0) return Ok();

        return BadRequest("Problem saving changes");
    }
}
