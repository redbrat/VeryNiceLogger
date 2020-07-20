using System.Collections.Generic;

public class MultilineCommentsParser : RecursiveParserBase
{
    public MultilineCommentsParser()
    {
    }

    protected override IEnumerator<CommandPositionPair> parseOpen(int position)
    {
        var isInsideComment = false;
        for (; position < _string.Length; position++)
        {
            var letter = _string[position];
            if (isInsideComment)
            {
                if (letter == '*' && _string[++position] == '/')/**/
                {
                    yield return new CommandPositionPair(position, Commands.Match);
                    goto ExitLoop;
                }
            }
            else
            {
                if (letter == '/' && _string[++position] == '*')
                    isInsideComment = true;
                else
                {
                    yield return new CommandPositionPair(Commands.Pass);
                    goto ExitLoop;
                }
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
