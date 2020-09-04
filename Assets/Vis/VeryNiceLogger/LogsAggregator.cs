using System;

public struct LogsAggregator
{
    private AggregatorEntry[] _entries;

    private readonly int _maxEntries;

    private int _currentIndex;

    public LogsAggregator(int maxEntries)
    {
        _maxEntries = maxEntries;
        _entries = new AggregatorEntry[maxEntries];
        _currentIndex = 0;
    }

    public void Aggregate(string str, params Func<string>[] args) => _entries[_currentIndex++ % _maxEntries] = new AggregatorEntry(str, args);

    private struct AggregatorEntry
    {
        private string _str;
        private Func<string>[] _args;

        public AggregatorEntry(string str, Func<string>[] args)
        {
            _str = str;
            _args = args;
        }
    }
}
