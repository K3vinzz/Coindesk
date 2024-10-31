using System;
using AutoMapper;
using CoindeskApi.Data;
using CoindeskApi.DTOs;
using CoindeskApi.Models;
using CoindeskApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoinDeskController : ControllerBase
{
    private readonly ICoinDeskRepository _repo;

    public CoinDeskController(ICoinDeskRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get all converted data.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CustomCoinDeskResponse>> GetConvertedData()
    {
        var coinDeskResponseData = await _repo.GetCoinDeskDataAsync();
        var currencies = await _repo.GetCurrenciesAsync();

        if (coinDeskResponseData == null) return NotFound("Fail to get data from api.coindesk.com");

        var result = new CustomCoinDeskResponse
        {
            UpdatedISO = coinDeskResponseData.Time?.UpdatedISO,
            Bpi = coinDeskResponseData.Bpi?.ToDictionary(
                bpi => bpi.Key,
                bpi => new CustomCurrencyInfo
                {
                    Code = bpi.Value.Code,
                    Rate = bpi.Value.Rate,
                    Name = currencies.FirstOrDefault(c => c.Code == bpi.Value.Code)?.Name ?? "Unknown"
                }
            )
        };

        return Ok(result);
    }

    /// <summary>
    /// Get all converted data by Code.
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<CustomCoinDeskResponse>> GetConvertedDataByCode(string code)
    {
        var coinDeskResponseData = await _repo.GetCoinDeskDataAsync();
        var currencies = await _repo.GetCurrenciesAsync();

        //if (coinDeskResponseData?.Bpi == null || !coinDeskResponseData.Bpi.ContainsKey(code)) return NotFound();
        if (!currencies.Any(c => String.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase))) return NotFound();
        else if (coinDeskResponseData == null) return NotFound("Fail to get data from api.coindesk.com");

        var result = new CustomCoinDeskResponse
        {
            UpdatedISO = coinDeskResponseData.Time?.UpdatedISO,
            Bpi = new Dictionary<string, CustomCurrencyInfo>()
        };

        result.Bpi[code.ToUpper()] = new CustomCurrencyInfo
        {
            Code = code.ToUpper(),
            Rate = coinDeskResponseData.Bpi!.ContainsKey(code.ToUpper()) ? coinDeskResponseData.Bpi![code.ToUpper()].Rate : "Unknown",
            Name = currencies.FirstOrDefault(c => String.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase))?.Name ?? "Unknown"
        };

        return Ok(result);
    }
}