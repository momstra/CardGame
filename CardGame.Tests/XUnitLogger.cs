using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace CardGame.Tests
{
	public class XUnitLoggerProvider : ILoggerProvider
	{
		private readonly ITestOutputHelper _output;

		public XUnitLoggerProvider(ITestOutputHelper output)
		{
			_output = output;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new XUnitLogger(_output, categoryName);
		}

		public void Dispose()
		{

		}
	}

	public class XUnitLogger : ILogger, IDisposable
	{
		private readonly ITestOutputHelper _output;
		private readonly string _category;

		public XUnitLogger(ITestOutputHelper output, string category)
		{
			_output = output;
			_category = category;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return this;
		}

		public void Dispose()
		{

		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			_output.WriteLine($"{_category} [{eventId}]: {formatter(state, exception)}");
		}
	}
}
