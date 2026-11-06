using System.Collections.Concurrent;

using Serilog.Core;
using Serilog.Events;

namespace InvoiceTracker.Api.IntegrationTests.Infrastructure;

public sealed class InMemoryLogSink : ILogEventSink
{
    public ConcurrentQueue<LogEvent> LogEvents { get; } = new();

    public void Emit(LogEvent logEvent) => LogEvents.Enqueue(logEvent);
}
