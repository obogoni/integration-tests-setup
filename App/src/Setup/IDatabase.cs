namespace IntegrationTesting.Setup;

public interface IDatabase
{
	string BaseName { get; }

	string? ConnectionString { get; set; }

	void RunMigrations();
}