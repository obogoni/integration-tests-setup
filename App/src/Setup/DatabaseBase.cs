namespace IntegrationTesting.Setup
{
	public abstract class DatabaseBase : IDatabase
	{
		public string BaseName { get; private set; } = null!;

		public string? ConnectionString { get; set; }

		public DatabaseBase(string dbName)
		{
			BaseName = dbName;
		}

		public abstract void RunMigrations();
	}
}