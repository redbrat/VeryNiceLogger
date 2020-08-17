using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class HybridParser
{
    public static NodeBase Parse(string text)
    {
        var parentNode = (RecursiveNodeBase)new CurlyBracketsNode();
        parseRecursively(text, 0, 0, 0, parentNode);
        return parentNode;
    }

    private static (int, int, int) parseRecursively(string text, int i, int currentLineStart, int currentLine, RecursiveNodeBase parentNode)
    {
        var isInsideSingleLineComment = false;
        var isInsideMultiLineComment = false;

        var isInsideString = false;
        var isInsideFormattedString = false;
        var formattedStringCurlyBracketsCount = 0;

        var sb = new StringBuilder();
        var stringDebugSb = new StringBuilder();
        var formattedStringDebugSb = new StringBuilder();
        for (; i < text.Length; i++)
        {
            if (currentLine > 72)
            {

            }

            var letter = text[i];

            if (letter == '\n')
            {
                currentLine++;
                currentLineStart = i;
            }

            if (isInsideSingleLineComment) //пропускаем однострочные комментарии
            {
                if (letter == '\n')
                    isInsideSingleLineComment = false;
                continue;
            }
            if (isInsideMultiLineComment) //пропускаем многострочные комментарии
            {
                if (letter == '*' && text[++i] == '/')
                    isInsideMultiLineComment = false;
                continue;
            }

            if (isInsideFormattedString)
            {
                formattedStringDebugSb.Append(letter);
                if (letter == '{' && text[i - 1] != '\\')
                {
                    if (text[++i] == '{')
                        continue;
                    else
                    {
                        i--;
                        formattedStringCurlyBracketsCount++;
                    }
                    continue;
                }
                else if (letter == '}' && text[i - 1] != '\\')
                {
                    if (text[++i] == '}')
                        continue;
                    i--;
                    formattedStringCurlyBracketsCount--;
                    continue;
                }
                else if (letter == '"' && text[i - 1] != '\\' && formattedStringCurlyBracketsCount == 0)
                {
                    isInsideFormattedString = false;
                    isInsideString = false;
                    continue;
                }
                else
                    continue;
            }
            //var a = $"sadfokop {  $"asd{ $"daopko{$"dsfkko{$"sdfgko"}"}"}"  }";
            if (isInsideString)
            {
                stringDebugSb.Append(letter);
                if (letter == '"' && text[i - 1] != '\\')
                    isInsideString = false;
                continue;
            }

            var currentRecursiveNode = default(RecursiveNodeBase);
            switch (letter)
            {
                case '/':
                    var nextLetter = text[++i];
                    if (nextLetter == '/')
                        isInsideSingleLineComment = true;
                    else if (nextLetter == '*')
                        isInsideMultiLineComment = true;
                    else
                        i--;
                    break;

                case '"':
                    isInsideString = true;
                    stringDebugSb.Clear();
                    if (text[i - 1] == '$') //на i > 0 не проверяем, потому что файл не может начинаться прямо сразу со строки.
                    {
                        isInsideFormattedString = true;
                        formattedStringDebugSb.Clear();
                    }
                    break;

                case ';':
                    sb.Append(letter);
                    var commandString = sb.ToString();
                    sb.Clear();
                    parentNode.Accept(new CommandNode() { ParentNode = parentNode, Command = commandString, EndIndex = i, EndLine = currentLine, EndPosition = i - currentLineStart });
                    break;
                case ',':
                    //sb.Append(letter);
                    var parameterString = sb.ToString();
                    sb.Clear();
                    parentNode.Accept(new ParameterNode() { ParentNode = parentNode, Parameter = parameterString, EndIndex = i, EndLine = currentLine, EndPosition = i - currentLineStart });
                    break;

                case '{':
                    currentRecursiveNode = new CurlyBracketsNode() { ParentNode = parentNode, PrecedingExpression = sb.ToString(), StartLine = currentLine, StartPosition = i - currentLineStart, StartIndex = i };
                    sb.Clear();
                    break;
                case '(':
                    currentRecursiveNode = new RoundBracketsNode() { ParentNode = parentNode, PrecedingExpression = sb.ToString(), StartLine = currentLine, StartPosition = i - currentLineStart, StartIndex = i };
                    sb.Clear();
                    break;
                case '[':
                    currentRecursiveNode = new SquareBracketsNode() { ParentNode = parentNode, PrecedingExpression = sb.ToString(), StartLine = currentLine, StartPosition = i - currentLineStart, StartIndex = i };
                    sb.Clear();
                    break;
                case '}':
                case ')':
                case ']':
                    parentNode.EndIndex = i;
                    parentNode.EndLine = currentLine;
                    parentNode.EndPosition = i - currentLineStart;
                    parentNode.EndingExpression = sb.ToString();
                    sb.Clear();
                    addRegexNodes();
                    return (i, currentLineStart, currentLine);
                default:
                    sb.Append(letter);
                    break;
            }

            if (currentRecursiveNode != default)
            {
                addRegexNodes();
                (i, currentLineStart, currentLine) = parseRecursively(text, ++i, currentLineStart, currentLine, currentRecursiveNode);
                parentNode.Accept(currentRecursiveNode);
            }
        }

        void addRegexNodes()
        {
            //var plainText = sb.ToString();
            //sb.Clear();
            var regexNodesList = new List<RegexNodeBase>();

            //var arr = plainText.Split(';');
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    regexNodesList.Add(new CommandNode());
            //}
            //Debug.Log($"plainText = {plainText}");

            for (int j = 0; j < regexNodesList.Count; j++)
                parentNode.Accept(regexNodesList[j]);
        }

        return (i, currentLineStart, currentLine);
    }
}
