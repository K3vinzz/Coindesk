using System;
using CoindeskApi.Data;
using CoindeskApi.Models;
using CoindeskApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoinDeskController : ControllerBase
{
    private readonly CoinDeskService _coinDeskService;
    private readonly CoinDbContext _context;

    public CoinDeskController(CoinDeskService coinDeskService, CoinDbContext context)
    {
        _coinDeskService = coinDeskService;
        _context = context;
    }

    /// <summary>
    /// Get all converted data.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetConvertedData()
    {
        var coinDeskData = await _coinDeskService.GetCoinDeskDataAsync();
        var currencies = await _context.Currencies.ToListAsync();

        var result = new
        {
            updatedISO = coinDeskData?.Time?.UpdatedISO.ToString("yyyy/MM/dd HH:mm:ss") ?? "Unknown",
            bpi = coinDeskData?.Bpi?.Select(bpi => new
            {
                Code = bpi.Value.Code,
                Name = currencies.FirstOrDefault(c => c.Code == bpi.Value.Code)?.Name ?? "Unknown",
                Rate = bpi.Value.Rate
            })
        };

        return Ok(result);
    }

    /// <summary>
    /// Get all converted data.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CoinDeskResponse>>> GetConvertedData2()
    {
        var coinDeskData = await _coinDeskService.GetCoinDeskDataAsync();
        var currencies = await _context.Currencies.ToListAsync();

        var result = new
        {
            updatedISO = coinDeskData?.Time?.UpdatedISO.ToString("yyyy/MM/dd HH:mm:ss") ?? "Unknown",
            bpi = coinDeskData?.Bpi?.Select(bpi => new
            {
                Code = bpi.Value.Code,
                Name = currencies.FirstOrDefault(c => c.Code == bpi.Value.Code)?.Name ?? "Unknown",
                Rate = bpi.Value.Rate
            })
        };

        var result2 = new
        {
            updatedISO = coinDeskData?.Time?.UpdatedISO.ToString("yyyy/MM/dd HH:mm:ss") ?? "Unknown",
            bpi = coinDeskData?.Bpi?.Select(bpi => new
            {
                Code = bpi.Value.Code,
                Name = currencies.FirstOrDefault(c => c.Code == bpi.Value.Code)?.Name ?? "Unknown",
                Rate = bpi.Value.Rate
            })
        };

        return Ok(result);
    }


    /// <summary>
    /// Get all converted data by Code.
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult> GetConvertedDataByCode(string code)
    {
        var coinDeskData = await _coinDeskService.GetCoinDeskDataAsync();
        var currencies = await _context.Currencies.ToListAsync();

        var bpiData = coinDeskData?.Bpi?
            .Where(bpi => bpi.Value.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
            .Select(bpi => new
            {
                Code = bpi.Value.Code,
                Name = currencies.FirstOrDefault(c => c.Code == bpi.Value.Code)?.Name ?? "Unknown",
                Rate = bpi.Value.Rate
            });

        if (bpiData == null || !bpiData.Any())
        {
            return NotFound(new { message = $"No data found for code '{code}'." });
        }

        var result = new
        {
            updatedISO = coinDeskData?.Time?.UpdatedISO.ToString("yyyy/MM/dd HH:mm:ss") ?? "Unknown",
            bpi = bpiData
        };

        return Ok(result);
    }
}