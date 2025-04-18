# .NET Integration Testing

This project aims to present an approach for implementing integration tests with .NET in legacy systems. Writing integration tests in ASP.NET Core is [pretty straightforward](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0), but when we're talking about legacy .NET Framework applications, it gets a bit trickier. So I wrote a few abstractions that would let me implement integration testing against a SQL Server (or any other SQL database really). Although this example is implemented with .NET 8, every dependency used targets at least .NET Standard 2.0.

## How it works

### Database migrations
In order to implement integration tests, you need to be able to easily create a new database from scratch for you application at any specific version. In the modern .NET stack usually you'd manage your database migrations with EF Core. In my case, though, migrations were implemented with plain SQL script files. So I'm using [DbUp](https://github.com/DbUp/DbUp) to run migrations and create the databases for testing. This can be done by implementing the IDatabase interface:

```c#
public class AppDatabase : IDatabase
{ 
	public void RunMigrations()
	{
		var upgrader = DeployChanges.To
                    .SqlDatabase(ConnectionString)
                    .WithScriptsEmbeddedInAssembly(typeof(DbConfiguration).Assembly)
                    .LogToTrace()
                    .Build();

		upgrader.PerformUpgrade();
	}
}
```
### Fixtures
Just like in my case, maybe your application (and therefore your tests) actually depends on multiple databases. For that reason, there's an ITestFixture interface where you can define a list of IDatabase objects and the logic to seed basic data needed for tests:
```c#
public class AppDatabase : IDatabase
{ 
	public interface ITestFixture
	{
		IReadOnlyList<IDatabase> Databases { get; }

		void SeedData();
	}
}
```
That way you can define multiple fixtures with the same databases but different seed data to keep your test suites decoupled from each other.
### Setup
With all the dependencies defined, all there's left to do is to make sure everything is set up right before your tests start to run:
```c#
[TestClass]
internal class Startup
{
	[AssemblyInitialize]
	public static void Initialize(TestContext context)
	{
		var options = new TestEnvironmentOptions(context);

		options.AddFixture<AppEnvironment>();

		TestEnvironment.Initialize(options);
	}

	[AssemblyCleanup]
	public static void Cleanup()
	{
		TestEnvironment.TearDown();
	}
}
```
The TestEnvironment.Initialize method creates all the databases on the configured SQL Server instance, generates the connection strings for each IDatabase object and runs the migration and seed data logic specific to your application. In this example, I'm using MSTest, but you should be able to adapt it to any testing framework you want. The TearDown method deletes all databases at the end.
### Testing with fixtures
Internally, the TestEnvironment class stores the fixture objects in a DI Container that is available during test execution. So grabbing a fixture for your tests should be easy with this base class for your test classes:
```c#
public abstract class IntegrationTestsBase<TFixture> where TFixture : class, ITestFixture
	{
		public TFixture Environment { get; private set; }

		public IntegrationTestsBase()
		{
			Environment = TestEnvironment.GetFixture<TFixture>()!;
		}
	}
```
### Configuration
The configuration for the SQL Server instance used needs to be set in the MSTest .runsettings file. Again, you could change that to a single config file if you don't want to use MSTest.

## Design choices

### Why not use Docker?
At first, using Docker with [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) to set up and discard all databases felt like the obvious choice. But in my case, I settled for a simpler solution. Basically, every developer in my team already had a SQL Server instance installed on their machine by default. So using Docker added a layer of complexity that wasn't necessary. It should be easy to spin up a SQL Server instance and modify the .runsettings file in a CI/CD pipeline as well. Also, if you're targeting older versions of SQL Server, you will have difficulty finding official docker images.

### Why not use an in-memory database with Entity Framework?
This approach limits what you can do in a lot of ways. First off, in my case we didn't even use EF to begin with. Second, even if that was the case, using an in-memory provider doesn't allow you to test raw SQL queries - and we had lots of them. Additionally, using a real database allows you to run tests as close as possible to the production environment. If your application targets a specific version of SQL Server, you can make sure your migration scripts and SQL queries have no compatibility issues by using an SQL Server instance with the same target version for your tests.

### Parallelism
Sharing the same external dependency between tests at the same time can cause a lot of unexpected errors. I tried to be as conservative as possible with this, so I disabled test parallelism for all integration tests by default. Using .runsettings and potentially Directory.Build.props files, I've created a convention to allow unit tests and integration tests to run with different configurations as long as they are kept in different projects. Basically, tests in projects with the .UnitTests suffix will run parallel while tests in projects ending with .IntegrationTests won't. You can see an example of that in the [UnitTestsExample](./UnitTestsExample/) folder.

### Test Isolation
I would argue that defining one concrete environment (ITestFixture) per test class is the best approach. That way each test suite is responsable for its own test environment. I write the base test class with that scenario in mind. Under the hood, each database will have a unique identifier prepended to its name to ensure there's no colision in the SQL Server instance for the same database. So you could create as many environments as you'd like.

## Conclusion

Hopefully, this approach offers a practical way to add valuable integration tests to your legacy .NET applications targeting SQL Server :)
