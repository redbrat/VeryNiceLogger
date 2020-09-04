using System.Text;

public class DebugProcessor : ICodeProcessor
{
    public void ProcessBlockEnd(CurlyBlock curlyBlock, StringBuilder sb)
    {
        sb.Insert(0, $"#Block {curlyBlock}: Line {curlyBlock.LineIndex} End#");
    }

    public void ProcessBlockStart(CurlyBlock curlyBlock, StringBuilder sb)
    {
        sb.Insert(0, $"#Block {curlyBlock}: Line {curlyBlock.LineIndex} Start#");
    }

    public void ProcessCommand(CurlyBlock curlyBlock, StringBuilder sb, int lineIndex)
    {
        sb.Insert(0, $"#Command of Block {curlyBlock}: Line {lineIndex}#");
    }
}