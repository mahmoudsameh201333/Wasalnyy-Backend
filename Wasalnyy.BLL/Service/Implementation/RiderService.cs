using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Rider;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class RiderService : IRiderService
    {
        private readonly IRiderRepo _riderRepo;
        private readonly IMapper _mapper;

        public RiderService(IRiderRepo riderRepo, IMapper mapper)
        {
            _riderRepo = riderRepo;
            _mapper = mapper;
        }
        public async Task<ReturnRiderDto?> GetByIdAsync(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id);
            if (rider == null)
                throw new NotFoundException($"Rider with ID '{id}' was not found.");

            return _mapper.Map<Rider, ReturnRiderDto>(rider);
        }

        public async Task<bool> IsRiderSuspended(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id);
           

            return rider.IsSuspended;

        }

        public async Task<string> RiderName(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id); 
            if (rider == null)
                return string.Empty; 

            return rider.FullName;

        }

        public async Task<string?> RiderProfileImage(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id);
            if (rider == null)
                return string.Empty;

            return rider.Image;
        }

        public async Task<int> RiderTotalTrips(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id);
            if (rider == null)
                return 0;

            return rider.Trips.Count();
        }

        public Task<decimal> RiderWalletBalance(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateRiderInfo(string id, RiderUpdateDto riderUpdate)
        {
            var oldriderinfos = await _riderRepo.GetByIdAsync(id);
            if (oldriderinfos == null)
            {
                return false;
            }

            _mapper.Map(riderUpdate, oldriderinfos);
            await _riderRepo.UpdateRiderAsync(oldriderinfos);
            await _riderRepo.SaveChangesAsync();
            return true;

        }

    }
}
