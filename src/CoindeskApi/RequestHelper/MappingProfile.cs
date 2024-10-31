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
        CreateMap<CurrencyDTO, Currency>();

        CreateMap<CoinDeskResponse, CustomCoinDeskResponse>()
            .ForMember(dest => dest.UpdatedISO, opt => opt.MapFrom(src => src.Time!.UpdatedISO))
            .ForMember(dest => dest.Bpi, opt => opt.MapFrom(src => src.Bpi));
        CreateMap<CurrencyInfo, CustomCurrencyInfo>();
    }
}
