namespace DbTestBed.Helper.Cli.Databases
{
	internal class Database
	{
		public string Name { get; set; } = null!;

		public decimal Size { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}