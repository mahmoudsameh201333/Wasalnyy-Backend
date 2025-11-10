namespace Wasalnyy.DAL.Entities
{
	public class User : IdentityUser
	{
		public string FullName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public int Age(){
			var today = DateTime.Today;
			int age = today.Year - DateOfBirth.Year;
			if (DateOfBirth.Date > today.AddYears(-age))
				age--;
			return age;
		}
		public string? Image { get; set; }
		public string PhoneNumber { get; set; }
		public Gender Gender { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsDeleted { get; protected set; } = false;

		public User()
		{
			
		}
		public User(string fullName, DateTime dateOfBirth,string phoneNumber, Gender gender)
		{
			FullName = fullName;
			DateOfBirth = dateOfBirth;
			PhoneNumber = phoneNumber;
			Gender = gender;
			CreatedAt = DateTime.Now;
		}
	}
}
