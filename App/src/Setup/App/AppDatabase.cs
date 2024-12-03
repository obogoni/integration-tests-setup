using DbUp;
using IntegrationTesting.Database;

namespace IntegrationTesting.Setup.App;

public class AppDatabase : DatabaseBase
{
	public AppDatabase()
		: base("MyApp")
	{
	}

	public override void RunMigrations()
	{
		var upgrader = DeployChanges.To
								.SqlDatabase(ConnectionString)
								.WithScriptsEmbeddedInAssembly(typeof(DbConfiguration).Assembly)
								.LogToTrace()
								.Build();

		upgrader.PerformUpgrade();
	}
}