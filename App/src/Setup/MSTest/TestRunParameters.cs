using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTesting.Setup.MSTest;

internal class TestRunParameters : ValueObject
{
	public string ServerName { get; set; }

	public string User { get; set; }

	public string Password { get; set; }

	public TestRunParameters(TestContext context)
	{
		Guard.Against.Null(context);

		ServerName = GetValue(context, nameof(ServerName));
		User = GetValue(context, nameof(User));
		Password = GetValue(context, nameof(Password));
	}

	private string GetValue(TestContext context, string property)
	{
		var value = context.Properties[property];

		Guard.Against.Null(value);

		return Guard.Against.NullOrEmpty(value.ToString());
	}

	protected override IEnumerable<IComparable> GetEqualityComponents()
	{
		yield return ServerName;

		yield return User;

		yield return Password;
	}
}