using System.Collections.Generic;

public class RootParser : RecursiveParserBase
{
    public RootParser(string @string, params RecursiveParserBase[] children) : base(@string, children)
    {
    }

    protected override IEnumerator<CommandPositionPair> parseOpen(int position)
    {
        yield return new CommandPositionPair(Commands.Match);
    }

    protected override IEnumerator<CommandPositionPair> parseClose(int position)
    {
        yield return new CommandPositionPair(Commands.Match);
    }
}
