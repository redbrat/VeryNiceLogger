using System;
using System.Collections.Generic;
using System.IO;
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

            _result = sdaiofj.Debug(_text);

            //_result = sb.ToString();
            //sb.Clear();
        }
        if (GUILayout.Button("Test files enumeration"))
        {
            var enumerator = FilesEnumratator.EnumerateFilesInProject(config);
            var i = 0;
            var listOfFiles = new List<string>();
            while (enumerator.MoveNext())
            {
                Debug.LogError($"enumerator file = {enumerator.Current}");
                listOfFiles.Add(enumerator.Current);

                i++;
            }
            Debug.LogError($"i = {i}");

            var processor = new DefaultProcessor();
            var listOfFailedParsing = new List<(string file, string error)>();
            for (i = 0; i < listOfFiles.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Simple Progress Bar", listOfFiles[i], i / (float)listOfFiles.Count);

                var text = File.ReadAllText(listOfFiles[i]);
                //try
                //{
                    var parsedText = RecursiveParserV2.Parse(text);
                    var processedText = parsedText.Process(processor, text);
                    File.WriteAllText(listOfFiles[i], processedText);
                //}
                //catch (Exception e)
                //{
                //    listOfFailedParsing.Add((text, e.Message));
                //}
            }
            EditorUtility.ClearProgressBar();
            Debug.Log($"listOfFailedParsing.Count = {listOfFailedParsing.Count}");
        }
    }
}
