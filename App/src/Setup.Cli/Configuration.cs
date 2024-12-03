using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace DbTestBed.Helper.Cli
{
	internal static class Configuration
	{
		public static string ConnectionString { get; private set; }

		static Configuration()
		{
			var builder = new ConfigurationBuilder();
			var exeDirectory = System.AppContext.BaseDirectory;

			builder.SetBasePath(exeDirectory);
			builder.AddJsonFile("appSettings.json", optional: false);

			var config = builder.Build();
			var configValue = config.GetConnectionString("Default");

			ConnectionString = Guard.Against.NullOrEmpty(configValue);
		}
	}
}