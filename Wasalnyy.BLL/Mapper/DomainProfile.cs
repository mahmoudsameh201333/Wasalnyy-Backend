using AutoMapper;
using Wasalnyy.BLL.DTO;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Pricing;
using Wasalnyy.BLL.DTO.Rider;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.BLL.DTO.Update;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Driver, ReturnDriverDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => src.ZoneId))
                .ForMember(dest => dest.License, opt => opt.MapFrom(src => src.License))
                .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src => src.Coordinates))
                .ForMember(dest => dest.DriverStatus, opt => opt.MapFrom(src => src.DriverStatus))
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle));

            CreateMap<Rider, ReturnRiderDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.RiderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

         CreateMap<UpdateRider,Rider>().ReverseMap();
            CreateMap<UpdateDriver, Driver>().ReverseMap();

            // Wallet mappings
            CreateMap<Wallet, WalletDto>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionDto>()
                .ForMember(dest => dest.TransactionType,
                           opt => opt.MapFrom(src => src.TransactionType.ToString()));



            CreateMap<RequestTripDto, Trip>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.PickupCoordinates, opt => opt.MapFrom(src => src.PickupCoordinates))
                .ForMember(dest => dest.DistinationCoordinates, opt => opt.MapFrom(src => src.DistinationCoordinates))
                .ForMember(dest => dest.RequestedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.TripStatus, opt => opt.MapFrom(_ => TripStatus.Requested))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

            CreateMap<Trip, CalculatePriceDto>()
                .ForMember(dest => dest.DistanceKm, opt => opt.MapFrom(src => src.DistanceKm))
                .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.DurationMinutes));


            CreateMap<Trip, TripDto>();

            CreateMap<CreateZoneDto, Zone>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src => src.Coordinates));

            CreateMap<Zone, ReturnZoneDto>();

            CreateMap<UpdateZoneDto, Zone>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Vehicle, VehicleDto>();
        }
    }
}
