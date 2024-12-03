using Microsoft.Extensions.Logging;

namespace IntegrationTesting.Setup.Logging
{
	public static class Extensions
	{
		public static void DatabaseCreated(this ILogger logger, string dbName, string connectionString)
		{
			logger.LogInformation("Database created: {0}", dbName);
			logger.LogInformation("Connection string: {0}", connectionString);
		}

		public static void DatabaseDropped(this ILogger logger, string dbName, string connectionString)
		{
			logger.LogInformation("Database droped: {0}", dbName);
			logger.LogInformation("Connection string: {0}", connectionString);
		}

		public static void MigrationsExecuted(this ILogger logger, TimeSpan elapsed)
		{
			logger.LogInformation("Elapsed time during migration scripts execution: {0:c}", elapsed);
		}
	}
}