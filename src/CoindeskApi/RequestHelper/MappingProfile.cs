using System;
using AutoMapper;
using CoindeskApi.DTOs;
using CoindeskApi.Models;

namespace CoindeskApi.RequestHelper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateCurrencyDTO, Currency>();
        CreateMap<Currency, CreateCurrencyDTO>();
        CreateMap<Currency, CurrencyDTO>();
    }
}
