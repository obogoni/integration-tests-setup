using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace IntegrationTesting.Setup.Logging;

public class TestContextLogger : ILogger
{
	private readonly TestContext _testContext;
	private readonly string _categoryName;

	public TestContextLogger(TestContext testContext, string categoryName)
	{
		_testContext = testContext;
		_categoryName = categoryName;
	}

	public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		var message = formatter(state, exception);

		_testContext.WriteLine($"[{logLevel}] {_categoryName}: {message}");

		if (exception != null)
		{
			_testContext.WriteLine(exception.ToString());
		}
	}

	private class NullScope : IDisposable
	{
		public static NullScope Instance { get; } = new NullScope();

		private NullScope()
		{
		}

		public void Dispose()
		{
		}
	}
}

public class TestContextLoggerProvider : ILoggerProvider
{
	private readonly TestContext _testContext;
	private readonly ConcurrentDictionary<string, TestContextLogger> _loggers = new ConcurrentDictionary<string, TestContextLogger>();

	public TestContextLoggerProvider(TestContext testContext)
	{
		_testContext = testContext;
	}

	public ILogger CreateLogger(string categoryName)
	{
		return _loggers.GetOrAdd(categoryName, name => new TestContextLogger(_testContext, name));
	}

	public void Dispose()
	{
		_loggers.Clear();
	}
}