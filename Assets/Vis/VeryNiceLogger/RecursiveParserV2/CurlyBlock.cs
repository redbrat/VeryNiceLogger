using System;
using System.Collections.Generic;
using System.Text;

public class CurlyBlock
{
    public CurlyBlockInfo Info;
    public readonly int StartIndex;
    public readonly int LineIndex;
    public int EndIndex;

    public string DebugRepresentationOfContents;

    private readonly CurlyBlock _parent;

    private IList<(int characterIndex, int lineIndex)> _commands = new List<(int characterIndex, int lineIndex)>();
    private IList<CurlyBlock> _childBlocks = new List<CurlyBlock>();

    public CurlyBlock(int startIndex, int lineIndex, CurlyBlock parent)
    {
        StartIndex = startIndex;
        LineIndex = lineIndex;
        _parent = parent;
    }

    public CurlyBlock FindNearestClassOrStruct()
    {
        if (Info.Class != default || Info.Struct != default)
            return this;
        if (_parent == default)
            return default;
        else return _parent.FindNearestClassOrStruct();
    }

    public CurlyBlock FindNearestProperty()
    {
        if (Info.Property != default)
            return this;
        if (_parent == default)
            return default;
        else return _parent.FindNearestProperty();
    }

    internal object FindNearestMethod()
    {
        if (Info.Method != default)
            return this;
        if (_parent == default)
            return default;
        else return _parent.FindNearestMethod();
    }

    public string Debug(string text)
    {
        var processor = new DebugProcessor();
        return Process(processor, text);
        //DebugRepresentationOfContents = text.Substring(StartIndex, EndIndex - StartIndex);

        //var nextChildBlockIndex = _childBlocks.Count - 1;
        //var nextCommandBlockIndex = _commands.Count - 1;
        //for (int i = EndIndex; i >= StartIndex; i--)
        //{
        //    var nextChildBlock = nextChildBlockIndex >= 0 ? _childBlocks[nextChildBlockIndex] : default;
        //    var nextCommand = nextCommandBlockIndex >= 0 ? _commands[nextCommandBlockIndex] : -1;

        //    if (i == EndIndex)
        //        sb.Insert(0, $"#Block {ToString()} End#");
        //    else if (i == StartIndex)
        //    {
        //        sb.Insert(0, $"#Block {ToString()} Start#");
        //        return i;
        //    }
        //    else if (i == nextCommand)
        //    {
        //        nextCommandBlockIndex--;
        //        sb.Insert(0, $"#Command of Block {ToString()}#");
        //    }
        //    else if (nextChildBlock != default && i == nextChildBlock.EndIndex)
        //    {
        //        nextChildBlockIndex--;
        //        i = nextChildBlock.Debug(sb, text);
        //    }
        //    sb.Insert(0, text[i]);
        //}
        //return -1;
    }

    public string Process(ICodeProcessor processor, string text)
    {
        var sb = new StringBuilder();
        processRecursively(processor, text, sb);
        var result = sb.ToString();
        sb.Clear();
        return result;
    }

    private int processRecursively(ICodeProcessor processor, string text, StringBuilder sb)
    {
        DebugRepresentationOfContents = text.Substring(StartIndex, EndIndex - StartIndex);

        var nextChildBlockIndex = _childBlocks.Count - 1;
        var nextCommandBlockIndex = _commands.Count - 1;
        for (int i = EndIndex; i >= StartIndex; i--)
        {
            var nextChildBlock = nextChildBlockIndex >= 0 ? _childBlocks[nextChildBlockIndex] : default;
            var (nextCommand, nextCommandLine) = nextCommandBlockIndex >= 0 ? _commands[nextCommandBlockIndex] : (-1, -1);

            if (i == EndIndex)
                processor.ProcessBlockEnd(this, sb);
            else if (i == StartIndex)
            {
                processor.ProcessBlockStart(this, sb);
                sb.Insert(0, text[i]);
                return i - 1;
            }
            else if (i == nextCommand)
            {
                nextCommandBlockIndex--;
                processor.ProcessCommand(this, sb, nextCommandLine);
            }
            else if (nextChildBlock != default && i == nextChildBlock.EndIndex)
            {
                nextChildBlockIndex--;
                i = nextChildBlock.processRecursively(processor, text, sb);
            }
            sb.Insert(0, text[i]);
        }
        return -1;
    }

    internal void AppendCommand(int characterIndex, int lineIndex) => _commands.Add((characterIndex, lineIndex));
    internal void AppendChildBlock(CurlyBlock newCurlyBlock) => _childBlocks.Add(newCurlyBlock);


    private class newChallenge<T> where T : IList<(string, int, IDictionary<IEnumerable<int>, (string, List<int>)>)>
    {
        private List<(int, string, IDictionary<IEnumerable<int>, (string, List<int>)>)> getMyList() { return default; }
    }

    private StringBuilder _sb = new StringBuilder();
    public override string ToString()
    {
        if (Info.Method != default)
        {
            _sb.Append($"Method {Info.Method}(");
            for (int i = 0; i < Info.Parameters.Count; i++)
            {
                _sb.Append(Info.Parameters[i].ToString());
                if (i < Info.Parameters.Count - 1)
                    _sb.Append(", ");
            }
            _sb.Append($")");
            if (Info.GenericNarrowing != default)
                _sb.Append(Info.GenericNarrowing);
        }
        else if (Info.Switch != default)
        {
            _sb.Append($"Switch (");
            for (int i = 0; i < Info.Parameters.Count; i++)
            {
                _sb.Append(Info.Parameters[i].ToString());
                if (i < Info.Parameters.Count - 1)
                    _sb.Append(", ");
            }
            _sb.Append($")");
        }
        else if (Info.If != default)
        {
            _sb.Append($"If (");
            for (int i = 0; i < Info.Parameters.Count; i++)
            {
                _sb.Append(Info.Parameters[i].ToString());
                if (i < Info.Parameters.Count - 1)
                    _sb.Append(", ");
            }
            _sb.Append($")");
        }
        else if (Info.Class != default)
        {
            _sb.Append($"Class {Info.Class}");
            if (Info.GenericNarrowing != default)
                _sb.Append(Info.GenericNarrowing);
        }
        else if (Info.Struct != default)
        {
            _sb.Append($"Struct {Info.Struct}");
            //if (Info.GenericNarrowing != default)
            //    _sb.Append(Info.GenericNarrowing);
        }
        else if (Info.Interface != default)
        {
            _sb.Append($"Interface {Info.Interface}");
            if (Info.GenericNarrowing != default)
                _sb.Append(Info.GenericNarrowing);
        }
        else if (Info.Enum != default)
            _sb.Append($"Enum {Info.Enum}");
        else if (Info.Namespace != default)
            _sb.Append($"Namespace {Info.Namespace}");
        else if (Info.Property != default)
            _sb.Append($"Property {Info.Property}");
        else if (Info.PropertyGet != default)
            _sb.Append($"PropertyGet {Info.PropertyGet}");
        else if (Info.PropertySet != default)
            _sb.Append($"PropertySet {Info.PropertySet}");
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
