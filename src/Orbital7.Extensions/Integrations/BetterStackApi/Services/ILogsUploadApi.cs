﻿namespace Orbital7.Extensions.Integrations.BetterStackApi;

public interface ILogsUploadApi
{
    Task LogEventAsync(
        string sourceToken,
        string ingestingHost,
        LogEvent logEvent);

    Task LogEventsAsync(
        string sourceToken,
        string ingestingHost,
        IEnumerable<LogEvent> logEvents);
}
