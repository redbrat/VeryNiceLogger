using System.Collections.Generic;

public class WhiteSpaceParser : RecursiveParserBase
{
    public WhiteSpaceParser()
    {
    }

    protected override IEnumerator<CommandPositionPair> parseOpen(int position)
    {
        for (; position < _string.Length; position++)
        {
            var letter = _string[position];
            var atLeastOne = false;
            switch (letter)
            {
                case '	':
                case ' ':
                    atLeastOne = true;
                    break;
                default:
                    if (atLeastOne)
                        yield return new CommandPositionPair(position, Commands.Match);
                    else
                        yield return new CommandPositionPair(Commands.Pass);
                    goto ExitLoop;
            }
        }
    ExitLoop:
        {

        }
    }

    protected override IEnumerator<CommandPositionPair> parseClose(int position)
    {
        yield return new CommandPositionPair(position, Commands.Match);
    }
}
