using Ardalis.GuardClauses;
using IntegrationTesting.Setup.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTesting.Setup
{
	public sealed class TestEnvironmentOptions
	{
		public TestContext TestContext { get; }

		internal IServiceCollection Services { get; }

		public TestEnvironmentOptions(TestContext context)
		{
			Guard.Against.Null(context);

			TestContext = context;
			Services = new ServiceCollection();

			Services.AddLogging(builder => builder
											.AddConsole()
											.AddProvider(new TestContextLoggerProvider(context)));
			Services.AddSingleton<TestEnvironment>();
		}

		public TestEnvironmentOptions AddFixture<TTestFixture>() where TTestFixture : class, ITestFixture
		{
			Services.AddScoped<ITestFixture, TTestFixture>();

			return this;
		}
	}
}