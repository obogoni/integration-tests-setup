namespace IntegrationTesting.Setup
{
	public interface ITestFixture
	{
		IReadOnlyList<IDatabase> Databases { get; }

		void SeedData();
	}
}