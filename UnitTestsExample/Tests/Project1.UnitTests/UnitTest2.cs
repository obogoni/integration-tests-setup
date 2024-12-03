namespace Project1.UnitTests;

[TestClass]
public class UnitTest2
{
	[TestMethod]
	public void TestMethod1()
	{
		Thread.Sleep(2000);
	}

	[AssemblyInitialize]
	public static void Init(TestContext context)
	{
	}
}