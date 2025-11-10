namespace Wasalnyy.DAL.Database
{
	public static class DbSeeder
	{
		public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			var roles = new[] { "Admin", "Driver", "Rider" };

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
					await roleManager.CreateAsync(new IdentityRole(role));
			}

			// Seed Admin User
			string adminEmail = "admin@wasalnyy.com";
			string adminPassword = "Admin@123";

			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				var newAdmin = new User
				{
					UserName = adminEmail,
					Email = adminEmail,
					FullName = "System Admin",
					PhoneNumber = "15555222",
					DateOfBirth = DateTime.Now,
					Gender = Gender.Male,
					EmailConfirmed = true
				};

				var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
				if (createAdmin.Succeeded)
					await userManager.AddToRoleAsync(newAdmin, "Admin");
			}
		}
	}
}
