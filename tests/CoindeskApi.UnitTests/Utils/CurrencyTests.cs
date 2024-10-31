using System;
using AutoFixture;
using AutoMapper;
using CoindeskApi.Controllers;
using CoindeskApi.Data;
using CoindeskApi.DTOs;
using CoindeskApi.Models;
using CoindeskApi.RequestHelper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CoindeskApi.UnitTests.Utils;

public class CurrencyTests
{
    private readonly Mock<ICurrenciesRepository> _repo;
    // fixture用於產生隨機測試資料
    private readonly Fixture _fixture;
    private readonly CurrenciesController _controller;
    private readonly IMapper _mapper;

    public CurrencyTests()
    {
        _fixture = new Fixture();
        _repo = new Mock<ICurrenciesRepository>();
        
        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfile).Assembly);
        }).CreateMapper().ConfigurationProvider;
        _mapper = new Mapper(mockMapper);

        _controller = new CurrenciesController(_repo.Object, _mapper);
    }

    [Fact]
    public async Task GetCurrencies_WithNoParams_Returns10Currencies()
    {
        // Given
        var currencies = _fixture.CreateMany<Currency>(10).ToList();
        _repo.Setup(repo => repo.GetCurrencies()).ReturnsAsync(currencies);
        
        // When
        var result = await _controller.GetCurrencies();

        // Then
        Assert.Equal(10, result.Value!.Count);
        Assert.IsType<ActionResult<List<Currency>>>(result);
    }

    [Fact]
    public async Task GetCurrencyById_WithValidId_ReturnsCurrency()
    {
        // Given
        var currency = _fixture.Create<CurrencyDTO>();
        _repo.Setup(repo => repo.GetCurrencyById(It.IsAny<int>())).ReturnsAsync(currency);
        
        // When
        var result = await _controller.GetCurrencyById(currency.Id);

        // Then
        Assert.Equal(currency.Code, result.Value!.Code);
        Assert.Equal(currency.Name, result.Value!.Name);
        Assert.IsType<ActionResult<CurrencyDTO>>(result);
    }

    [Fact]
    public async Task GetCurrencyById_WithInvalidId_ReturnsNotFound()
    {
        // Given
        _repo.Setup(repo => repo.GetCurrencyById(It.IsAny<int>()))!.ReturnsAsync(value: null);

        // When
        var result = await _controller.GetCurrencyById(123);
    
        // Then
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCurrencyByCode_WithValidCode_ReturnCurrency()
    {
        // Given
        var currency = _fixture.Create<CurrencyDTO>();
        _repo.Setup(repo => repo.GetCurrencyByCode(It.IsAny<string>())).ReturnsAsync(currency);
    
        // When
        var result = await _controller.GetCurrencyByCode(currency.Code);
    
        // Then
        Assert.Equal(currency.Name, result.Value!.Name);
        Assert.IsType<ActionResult<CurrencyDTO>>(result);
    }

    [Fact]
    public async Task GetCurrencyByCode_WithInvalidCode_ReturnNotFound()
    {
        // Given
        _repo.Setup(repo => repo.GetCurrencyByCode(It.IsAny<string>()))!.ReturnsAsync(value: null);
    
        // When
        var result = await _controller.GetCurrencyByCode("test");
    
        // Then
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddCurrency_WithValidCreateCurrencyDTO_ReturnsCreatedAtAction()
    {
        // Given
        var currency = _fixture.Create<CreateCurrencyDTO>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _controller.AddCurrency(currency);
        var createdResult = result.Result as CreatedAtActionResult;

        // Then
        Assert.NotNull(createdResult);
        Assert.Equal("GetCurrencyById", createdResult.ActionName);
        Assert.IsType<CurrencyDTO>(createdResult.Value);
    }

    [Fact]
    public async Task AddCurrency_FailedSave_ReturnsCreatedAtAction()
    {
        // Given
        var currency = _fixture.Create<CreateCurrencyDTO>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

        // When
        var result = await _controller.AddCurrency(currency);

        // Then
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCurrencyById_WithValidId_ReturnsOk()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        var updateCurrencyDTO = _fixture.Create<UpdateCurrencyDTO>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _controller.UpdateCurrencyById(currency.Id, updateCurrencyDTO);
    
        // Then
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyById_WithInvalidId_ReturnsNotFound()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        var updateCurrencyDTO = _fixture.Create<UpdateCurrencyDTO>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(value: null);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);
        
        // When
        var result = await _controller.UpdateCurrencyById(123, updateCurrencyDTO);

        // Then
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyById_WithSameCodeAndSameName_ReturnsNoContent()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        var updateCurrencyDTO = new UpdateCurrencyDTO
        {
            Code = currency.Code,
            Name = currency.Name
        };
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);
        
        // When
        var result = await _controller.UpdateCurrencyById(currency.Id, updateCurrencyDTO);

        // Then
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyById_FailedSave_ReturnsBadRequest()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        var updateCurrencyDTO = _fixture.Create<UpdateCurrencyDTO>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);
        
        // When
        var result = await _controller.UpdateCurrencyById(currency.Id, updateCurrencyDTO);

        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyByCode_WithValidCodeAndName_ReturnsOk()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _controller.UpdateCurrencyByCode(currency.Code, "test");
    
        // Then
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyByCode_WithInvalidCode_ReturnsNotFound()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(value: null);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);
        
        // When
        var result = await _controller.UpdateCurrencyByCode("test", currency.Name!);

        // Then
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyByCode_WithSameName_ReturnsNoContent()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.AddCurrency(currency));
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);
        
        // When
        var result = await _controller.UpdateCurrencyByCode(currency.Code, currency.Name!);

        // Then
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCurrencyByCode_FailedSave_ReturnsBadRequest()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.AddCurrency(It.IsAny<Currency>()));
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);
        
        // When
        var result = await _controller.UpdateCurrencyByCode(currency.Code, "test");

        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RemoveCurrencyById_WithValidId_ReturnsOK()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.RemoveCurrency(currency));
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _controller.RemoveCurrencyById(currency.Id);

        // Then
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task RemoveCurrencyById_WithInvalidId_ReturnsNotFound()
    {
        // Given
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(value: null);

        // When
        var result = await _controller.RemoveCurrencyById(123);

        // Then
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveCurrencyById_FailedSave_ReturnsBadRequest()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.RemoveCurrency(currency));
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

        // When
        var result = await _controller.RemoveCurrencyById(currency.Id);

        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RemoveCurrencyByCode_WithValidCode_ReturnsOK()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.RemoveCurrency(currency));
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _controller.RemoveCurrencyByCode(currency.Code);

        // Then
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task RemoveCurrencyByCode_WithInvalidCode_ReturnsNotFound()
    {
        // Given
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(value: null);

        // When
        var result = await _controller.RemoveCurrencyByCode("test");

        // Then
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveCurrencyByCode_FailedSave_ReturnsBadRequest()
    {
        // Given
        var currency = _fixture.Create<Currency>();
        _repo.Setup(repo => repo.GetCurrencyModelByCode(It.IsAny<string>())).ReturnsAsync(currency);
        _repo.Setup(repo => repo.RemoveCurrency(currency));
        _repo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

        // When
        var result = await _controller.RemoveCurrencyByCode(currency.Code);

        // Then
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
