namespace IntegrationTesting.Setup
{
	public abstract class IntegrationTestsBase<TFixture> where TFixture : class, ITestFixture
	{
		public TFixture Environment { get; private set; }

		public IntegrationTestsBase()
		{
			Environment = TestEnvironment.GetFixture<TFixture>()!;
		}
	}
}