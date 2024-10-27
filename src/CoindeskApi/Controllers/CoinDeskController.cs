using System;
using CoindeskApi.Data;
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
            updatedISO = coinDeskData.Time?.UpdatedISO.ToString("yyyy/MM/dd HH:mm:ss"),
            bpi = coinDeskData.Bpi?.Select(bpi => new
            {
                Code = bpi.Value.Code,
                Name = currencies.FirstOrDefault(c => c.Code == bpi.Value.Code)?.Name ?? "Unknown",
                Rate = bpi.Value.Rate
            })
        };

        return Ok(result);
    }
}