using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FacebookApi.Core.Tools
{
    public class Logger : ILogger
    {
        private List<Exception> _exceptions = new List<Exception>();

        public IReadOnlyList<Exception> Exceptions => _exceptions.ToImmutableList();

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (exception == null) return;
            _exceptions.Add(exception);
        }
    }
}