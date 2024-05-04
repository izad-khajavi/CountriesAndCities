using AutoMapper;
using CountriesAndCities.DTOs;
using CountriesAndCities.Models;

namespace CountriesAndCities.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Country, CountryDto>();
            CreateMap<CountryDto, Country>()
                .ForMember(dest => dest.CountryId, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PersianName, opt => opt.MapFrom(src => src.PersianName))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
            

            CreateMap<City, CityDto>();
            CreateMap<CityDto, City>()
                .ForMember(dest => dest.CityId, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PersianName, opt => opt.MapFrom(src => src.PersianName))
                .ForMember(dest => dest.CityCode, opt => opt.MapFrom(src => src.CityCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));
        }
    }
}
