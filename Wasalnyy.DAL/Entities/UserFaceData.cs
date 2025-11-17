namespace Wasalnyy.DAL.Entities
{
	public class UserFaceData
	{
		public Guid Id { get; set; }
		public string DriverId { get; set; }
		public Driver Driver { get; set; }

		public byte[] Embedding { get; set; }  // 128 floats as byte[]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
