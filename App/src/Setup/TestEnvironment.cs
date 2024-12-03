using Ardalis.GuardClauses;
using DbUp;
using IntegrationTesting.Setup.Logging;
using IntegrationTesting.Setup.MSTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IntegrationTesting.Setup;

public class TestEnvironment
{
	private static IServiceProvider? serviceProvider;
	private ILogger<TestEnvironment> logger;

	public TestEnvironment(ILogger<TestEnvironment> logger)
	{
		this.logger = logger;
	}

	public static void Initialize(TestEnvironmentOptions options)
	{
		Guard.Against.Null(options);

		serviceProvider = options.Services.BuildServiceProvider();

		var fixtures = serviceProvider.GetServices<ITestFixture>();
		var testEnvironment = serviceProvider.GetRequiredService<TestEnvironment>();
		var parameters = new TestRunParameters(options.TestContext);

		foreach (var fixture in fixtures)
		{
			testEnvironment.SetupFixture(fixture, parameters);
		}
	}

	public static void TearDown()
	{
		if (serviceProvider == null) return;

		var fixtures = serviceProvider.GetServices<ITestFixture>();
		var testEnvironment = serviceProvider.GetRequiredService<TestEnvironment>();

		foreach (var fixture in fixtures)
		{
			testEnvironment.TearDownFixture(fixture);
		}
	}

	public static TTestFixture? GetFixture<TTestFixture>() where TTestFixture : class, ITestFixture
	{
		Guard.Against.Null(serviceProvider);

		var services = serviceProvider.GetServices<ITestFixture>();

		foreach (var service in services)
		{
			if (service is TTestFixture fixture) return fixture;
		}

		return null;
	}

	private void SetupFixture(ITestFixture fixture, TestRunParameters parameters)
	{
		SetupDatabaseConnections(fixture.Databases, parameters, Guid.NewGuid());

		foreach (var db in fixture.Databases)
		{
			InitializeDatabase(db);
		}

		fixture.SeedData();
	}

	private void TearDownFixture(ITestFixture fixture)
	{
		foreach (var db in fixture.Databases)
		{
			CleanupDatabase(db);
		}
	}

	private void InitializeDatabase(IDatabase database)
	{
		Guard.Against.NullOrEmpty(database.ConnectionString);

		EnsureDatabase.For.SqlDatabase(database.ConnectionString);

		logger.DatabaseCreated(database.BaseName, database.ConnectionString);

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		database.RunMigrations();

		stopwatch.Stop();

		logger.MigrationsExecuted(stopwatch.Elapsed);
	}

	private void CleanupDatabase(IDatabase database)
	{
		Guard.Against.NullOrEmpty(database.ConnectionString);

		DropDatabase.For.SqlDatabase(database.ConnectionString);

		logger.DatabaseDropped(database.BaseName, database.ConnectionString);
	}

	private string GetDatabaseFullName(IDatabase database, Guid environmentUid)
	{
		return $"{database.BaseName}_{environmentUid:N}";
	}

	private string GenerateConnectionString(IDatabase database, TestRunParameters parameters, Guid environmentUid)
	{
		var fullDbName = GetDatabaseFullName(database, environmentUid);

		return $"Server={parameters.ServerName};Database={fullDbName};User Id={parameters.User};Password={parameters.Password};Encrypt=True;TrustServerCertificate=True;";
	}

	private void SetupDatabaseConnections(IEnumerable<IDatabase> databases, TestRunParameters parameters, Guid environmentUid)
	{
		foreach (var db in databases)
		{
			db.ConnectionString = GenerateConnectionString(db, parameters, environmentUid);
		}
	}
}