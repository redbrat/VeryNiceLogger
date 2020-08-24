using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TestView : ViewBase<Config>
{
    private string _text;
    private string _result;




    /* m */
    //

    public override void Render(Config config)
    {
        EditorGUILayout.LabelField("Test string:");
        _text = EditorGUILayout.TextArea(_text, GUILayout.Height(400));
        _result = EditorGUILayout.TextArea(_result, GUILayout.Height(400));
        if (GUILayout.Button("Test"))
        {
            //var parser = new RootParser(_text, new WhiteSpaceParser(), new SingleLineCommentsParser(), new MultilineCommentsParser());
            //var enumarator = parser.TryMatch(0);
            //while (enumarator.MoveNext())
            //{

            //}

            //var sdaiofj = (CurlyBracketsNode)HybridParser.Parse(_text);
            //_result = processCurly(sdaiofj, _text, _text);

            var sdaiofj = RecursiveParserV2.Parse(_text);

            var sb = new StringBuilder();
            sdaiofj.Fill(sb, _text);
            _result = sb.ToString();
            sb.Clear();

            //_result = sb.ToString();
            //sb.Clear();
        }
        if (GUILayout.Button("Test files enumeration"))
        {
            var enumerator = FilesEnumratator.EnumerateFilesInProject(config);
            var i = 0;
            while(enumerator.MoveNext())
            {
                Debug.LogError($"enumerator file = {enumerator.Current}");
                //EditorUtility.DisplayProgressBar("Simple Progress Bar", "Shows a progress bar for the given seconds", enumerator.Current.Item2 / (float)enumerator.Current.Item3);
                i++;
            }
            Debug.LogError($"i = {i}");
            //EditorUtility.ClearProgressBar();
        }
    }

    private string processCurly(CurlyBracketsNode curly, string result, string input)
    {
        for (int i = curly.ChildNodes.Count - 1; i >= 0; i--)
        {
            if (curly.ChildNodes[i] is CurlyBracketsNode)
                result = processCurly(curly.ChildNodes[i] as CurlyBracketsNode, result, input);
            else if (curly.ChildNodes[i] is CommandNode)
                result = processCommand(curly.ChildNodes[i] as CommandNode, result, input);
        }

        //Debug.Log($"curly.PrecedingExpression = {curly.PrecedingExpression}");

        if (curly.ChildNodes.Count > 0 && curly.IsBlocksContainer)
            result = $"{result.Substring(0, curly.StartIndex + 1)}MyLog.LogBlock({curly.StartLine}, {curly.StartPosition});{result.Substring(curly.StartIndex + 1)}";

        return result;
    }

    private string processCommand(CommandNode command, string result, string input) => $"{result.Substring(0, command.EndIndex + 1)}MyLog.LogCommand({command.EndLine}, {command.EndPosition});{result.Substring(command.EndIndex + 1)}";
}
