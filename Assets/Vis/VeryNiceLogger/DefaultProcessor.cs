using System.Text;

public class DefaultProcessor : ICodeProcessor
{
    public void ProcessBlockEnd(CurlyBlock curlyBlock, StringBuilder sb)
    {

    }

    public void ProcessBlockStart(CurlyBlock curlyBlock, StringBuilder sb)
    {
        var info = curlyBlock.Info;
        if (info.Method != default)
        {
            if (curlyBlock.FindNearestClassOrStruct() == default)
            {

            }
            sb.Insert(0, $"VNLogger.Log($\"Method { info.Method } of class {curlyBlock.FindNearestClassOrStruct().Info.Class} (line №{curlyBlock.StartIndex})\");");
        }
        else if (info.PropertyGet != default)
        {
            if (curlyBlock.FindNearestClassOrStruct() == default)
            {

            }
            sb.Insert(0, $"VNLogger.Log($\"Property getter { curlyBlock.FindNearestProperty() } of class {curlyBlock.FindNearestClassOrStruct().Info.Class} (line №{curlyBlock.StartIndex})\");");
        }
        else if (info.PropertySet != default)
            sb.Insert(0, $"VNLogger.Log($\"Property setter { curlyBlock.FindNearestProperty() } of class {curlyBlock.FindNearestClassOrStruct().Info.Class} (line №{curlyBlock.StartIndex})\");");
    }

    public void ProcessCommand(CurlyBlock curlyBlock, StringBuilder sb, int lineIndex)
    {
        //sb.Insert(0, $"VNLogger.Log($\"Command of class {curlyBlock.FindNearestClass().Info.Class} (line №{curlyBlock.StartIndex})\");");
        sb.Insert(0, $"VNLogger.Log({lineIndex});");
    }
}