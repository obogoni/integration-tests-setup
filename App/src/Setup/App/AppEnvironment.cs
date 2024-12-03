using Microsoft.Extensions.Logging;

namespace IntegrationTesting.Setup.App
{
	public class AppEnvironment : ITestFixture
	{
		private ILogger<AppEnvironment> logger;

		public IReadOnlyList<IDatabase> Databases { get; private set; }

		public AppDatabase AppDatabase { get; }

		public AppEnvironment(ILogger<AppEnvironment> logger)
		{
			this.logger = logger;

			AppDatabase = new AppDatabase();

			var databases = new List<IDatabase>();

			databases.Add(AppDatabase);

			Databases = databases.AsReadOnly();
		}

		public void SeedData()
		{
			//Insert default system data here
		}
	}
}