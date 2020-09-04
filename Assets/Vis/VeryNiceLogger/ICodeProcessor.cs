using System.Text;

public interface ICodeProcessor
{
    void ProcessBlockEnd(CurlyBlock curlyBlock, StringBuilder sb);
    void ProcessBlockStart(CurlyBlock curlyBlock, StringBuilder sb);
    void ProcessCommand(CurlyBlock curlyBlock, StringBuilder sb, int lineIndex);
}