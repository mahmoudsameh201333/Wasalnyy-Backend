using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
	public interface IUserFaceDataRepo
	{
		Task AddAsync(UserFaceData faceData);
		Task<List<UserFaceData>> GetAllDriversAsync();
		Task<UserFaceData?> GetByUserIdAsync(string userId);
	}
}
