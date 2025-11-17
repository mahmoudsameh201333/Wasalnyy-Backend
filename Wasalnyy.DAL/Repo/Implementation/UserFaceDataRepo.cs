using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
	public class UserFaceDataRepo : IUserFaceDataRepo
	{
		private readonly WasalnyyDbContext _context;
		public UserFaceDataRepo(WasalnyyDbContext context) => _context = context;

		public async Task AddAsync(UserFaceData faceData)
		{
			_context.UserFaceData.Add(faceData);
			await _context.SaveChangesAsync();
		}

		public async Task<List<UserFaceData>> GetAllDriversAsync()
			=> await _context.UserFaceData.Include(f => f.Driver).ToListAsync();

		public async Task<UserFaceData?> GetByUserIdAsync(string driverId)
			=> await _context.UserFaceData.FirstOrDefaultAsync(f => f.DriverId == driverId);
	}
}
