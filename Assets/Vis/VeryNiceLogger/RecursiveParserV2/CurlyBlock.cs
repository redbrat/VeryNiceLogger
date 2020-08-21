using System;
using System.Collections.Generic;
using System.Text;

public class CurlyBlock
{
    public CurlyBlockInfo Info;
    public int StartIndex;
    public int EndIndex;

    public string DebugRepresentationOfContents;

    private IList<int> _commands = new List<int>();
    private IList<CurlyBlock> _childBlocks = new List<CurlyBlock>();

    public int Fill(StringBuilder sb, string text)
    {
        DebugRepresentationOfContents = text.Substring(StartIndex, EndIndex - StartIndex);

        var nextChildBlockIndex = _childBlocks.Count - 1;
        var nextCommandBlockIndex = _commands.Count - 1;
        for (int i = EndIndex; i >= StartIndex; i--)
        {
            var nextChildBlock = nextChildBlockIndex >= 0 ? _childBlocks[nextChildBlockIndex] : default;
            var nextCommand = nextCommandBlockIndex >= 0 ? _commands[nextCommandBlockIndex] : -1;

            if (i == EndIndex)
                sb.Insert(0, $"#Block {ToString()} End#");
            else if (i == StartIndex)
            {
                sb.Insert(0, $"#Block {ToString()} Start#");
                return i;
            }
            else if (i == nextCommand)
            {
                nextCommandBlockIndex--;
                sb.Insert(0, $"#Command of Block {ToString()}#");
            }
            else if (nextChildBlock != default && i == nextChildBlock.EndIndex)
            {
                nextChildBlockIndex--;
                i = nextChildBlock.Fill(sb, text);
            }
            sb.Insert(0, text[i]);
        }
        return -1;
    }

    internal void AppendCommand(int commandIndex) => _commands.Add(commandIndex);
    internal void AppendChildBlock(CurlyBlock newCurlyBlock) => _childBlocks.Add(newCurlyBlock);

    private StringBuilder _sb = new StringBuilder();
    public override string ToString()
    {
        if (Info.Method != default)
        {
            _sb.Append($"Method {Info.Method} (");
            for (int i = 0; i < Info.Parameters.Count; i++)
            {
                _sb.Append(Info.Parameters[i].ToString());
                if (i < Info.Parameters.Count - 1)
                    _sb.Append(", ");
            }
            _sb.Append($")");
        }
        else if (Info.Class != default)
            _sb.Append($"Class {Info.Class}");
        else if (Info.Namespace != default)
            _sb.Append($"Namespace {Info.Namespace}");
        else if (Info.IsLambda)
        {
            _sb.Append($"Lambda (");
            for (int i = 0; i < Info.Parameters.Count; i++)
            {
                _sb.Append(Info.Parameters[i].ToString());
                if (i < Info.Parameters.Count - 1)
                    _sb.Append(", ");
            }
            _sb.Append($")");
        }

        var result = _sb.ToString();
        _sb.Clear();
        return result;
    }
}
