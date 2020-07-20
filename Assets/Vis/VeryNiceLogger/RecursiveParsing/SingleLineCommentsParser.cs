using System.Collections.Generic;

public class SingleLineCommentsParser : RecursiveParserBase
{
    public SingleLineCommentsParser()
    {
    }

    protected override IEnumerator<CommandPositionPair> parseOpen(int position)
    {
        var _isInsideComment = false;
        for (; position < _string.Length; position++)
        {
            var letter = _string[position];
            if (_isInsideComment)
            {
                if (letter == '\n')
                {
                    yield return new CommandPositionPair(position, Commands.Match);
                    goto ExitLoop;
                }
            }
            else
            {
                if (letter == '/')
                {
                    if (_string[++position] == '/')
                        _isInsideComment = true;
                    else
                    {
                        yield return new CommandPositionPair(Commands.Pass);
                        goto ExitLoop;
                    }
                }
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
