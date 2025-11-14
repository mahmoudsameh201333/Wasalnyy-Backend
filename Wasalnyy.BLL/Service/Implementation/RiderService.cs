using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;
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
        }
        public async Task<ReturnDriverDto?> GetByIdAsync(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id);
            if (rider == null)
                throw new NotFoundException($"Rider with ID '{id}' was not found.");

            return _mapper.Map<Rider, ReturnDriverDto>(rider);
        }
    }
}
