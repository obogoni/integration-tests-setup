using IntegrationTesting.Setup;
using IntegrationTesting.Setup.App;

namespace IntegrationTesting.App.IntegrationTests;

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