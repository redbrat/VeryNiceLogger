using System;

public static class VNLogger
{
    private const int _actionsCount = 100;

    private static LogsAggregator _logsAggregator = new LogsAggregator(_actionsCount);

    //public static void Log(string str, params Func<string>[] args) => _logsAggregator.Aggregate(str, args);
    public static void Log(string str, params Func<string>[] args) { }
    public static void Log(int id, params Func<string>[] args) { }
}
