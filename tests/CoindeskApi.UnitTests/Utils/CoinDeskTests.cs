using System;
using AutoFixture;
using CoindeskApi.Controllers;
using CoindeskApi.Data;
using CoindeskApi.Models;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace CoindeskApi.UnitTests.Utils;

public class CoinDeskTests
{
    private readonly Mock<ICoinDeskRepository> _repo;
    private readonly Fixture _fixture;

    private readonly CoinDeskController _controller;

    public CoinDeskTests()
    {
        _fixture = new Fixture();
        _repo = new Mock<ICoinDeskRepository>();
        _controller = new CoinDeskController(_repo.Object);
    }

    [Fact]
    public async Task GetConvertedData_ReturnsAllConvertedData()
    {
        // Given
        var coinDeskData = new CoinDeskResponse
        {
            Time = new TimeInfo { UpdatedISO = DateTimeOffset.UtcNow },
            Bpi = new Dictionary<string, CurrencyInfo>
            {
                { "USD", new CurrencyInfo { Code = "USD", Rate = "71,016.163" } },
                { "EUR", new CurrencyInfo { Code = "EUR", Rate = "65,358.163" } }
            }
        };
        _repo.Setup(repo => repo.GetCoinDeskDataAsync()).ReturnsAsync(coinDeskData);

        var currencies = new List<Currency>
        {
            new Currency { Code = "USD", Name = "美元" },
            new Currency { Code = "EUR", Name = "歐元" }
        };
        _repo.Setup(repo => repo.GetCurrenciesAsync()).ReturnsAsync(currencies);

        // When
        var result = await _controller.GetConvertedData();

        // Then
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CustomCoinDeskResponse>(okResult.Value);
        Assert.Equal(coinDeskData.Time.UpdatedISO, response.UpdatedISO);
        Assert.Equal(2, response.Bpi!.Count);
        Assert.Equal("美元", response.Bpi["USD"].Name);
        Assert.Equal("歐元", response.Bpi["EUR"].Name);
    }

    [Fact]
    public async Task GetConvertedData_WithNoResponseFromCoinDeskApi_ReturnsNotFound()
    {
        // Given
        _repo.Setup(repo => repo.GetCoinDeskDataAsync()).ReturnsAsync(value: null);

        // When
        var result = await _controller.GetConvertedData();

        // Then
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }


    [Fact]
    public async Task GetConvertedDataByCode_WithValidCode_ReturnsCorrectData()
    {
        // Given
        var coinDeskData = _fixture.Build<CoinDeskResponse>()
            .With(c => c.Bpi, new Dictionary<string, CurrencyInfo>
            {
                { "USD", _fixture.Build<CurrencyInfo>().With(c => c.Code, "USD").With(c => c.Rate, "71,016.163").Create() }
            })
            .With(c => c.Time, new TimeInfo { UpdatedISO = DateTimeOffset.UtcNow })
            .Create();
            
        var currencies = new List<Currency>
        {
            new Currency { Code = "USD", Name = "美元" }
        };

        _repo.Setup(repo => repo.GetCoinDeskDataAsync()).ReturnsAsync(coinDeskData);
        _repo.Setup(repo => repo.GetCurrenciesAsync()).ReturnsAsync(currencies);

        // When
        var result = await _controller.GetConvertedDataByCode("USD");

        // Then
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CustomCoinDeskResponse>(okResult.Value);
        
        Assert.Equal(coinDeskData.Time!.UpdatedISO, response.UpdatedISO);
        Assert.Single(response.Bpi!);
        Assert.Equal("美元", response.Bpi!["USD"].Name);
        Assert.Equal("71,016.163", response.Bpi["USD"].Rate);
    }
    
    [Fact]
    public async Task GetConvertedDataByCode_WithInvalidCode_ReturnsNotFound()
    {
        // Given
        var invalidCode = "GBP";
        var coinDeskData = _fixture.Build<CoinDeskResponse>()
            .With(c => c.Bpi, new Dictionary<string, CurrencyInfo>
            {
                { "USD", _fixture.Build<CurrencyInfo>().With(c => c.Code, "USD").With(c => c.Rate, "71,016.163").Create() }
            })
            .With(c => c.Time, new TimeInfo { UpdatedISO = DateTimeOffset.UtcNow })
            .Create();
        var currencies = new List<Currency>
        {
            new Currency { Code = "USD", Name = "美元" }
        };

        _repo.Setup(repo => repo.GetCoinDeskDataAsync()).ReturnsAsync(coinDeskData);
        _repo.Setup(repo => repo.GetCurrenciesAsync()).ReturnsAsync(currencies);
        
        // When
        var result = await _controller.GetConvertedDataByCode(invalidCode);

        // Then
        Assert.IsType<NotFoundResult>(result.Result);
    }
    

}
