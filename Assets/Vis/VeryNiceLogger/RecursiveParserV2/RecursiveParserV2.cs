using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class RecursiveParserV2
{
    public static IEnumerable<int> Parse(string text)
    {
        var result = new List<int>();
        parseBlockRecursively(text, 0, result);
        return result;
    }

    private static int parseBlockRecursively(string text, int i, IList<int> result)
    {
        var parameters = parseParameters(text, i - 2);

        if (parameters.Count > 2)
        {

        }

        for (; i < text.Length; i++)
        {
            var letter = text[i];

            switch (letter)
            {
                case '"':
                    i = parseString(text, i);
                    break;
                case '/':
                    if (text[i + 1] == '/')
                        i = parseSingleLineComment(text, i);
                    else if (text[i + 1] == '*')
                        i = parseMultiLineComment(text, i);
                    break;

                case '{':
                    i = parseBlockRecursively(text, i + 1, result);
                    break;
                case '}':
                    return i;
                case ';':
                    result.Add(i);
                    break;

                default:
                    break;
            }
        }
        return -1;
    }

    private static IList<Parameter> parseParameters(string text, int i)
    {
        var result = new List<Parameter>();

        //Action a = () => { };
        //Action<int> b = c => { };
        //Action<int, string> c = (d, e) =>
        ////asdasd
        ///*
        // * sadsad
        // */
        //{ };

        var arrowEncountered = false;
        var isInsideComment = false;
        var isInsideGenericsOutsideRoundBrackets = 0;
        var isInsideGenericsInsideRoundBrackets = 0;
        var isInsideRoundBrackets = false;
        var sb = new StringBuilder();
        for (; i >= 0; i--)
        {
            var letter = text[i];

            if (isInsideComment)
            {
                if (letter == '*' && text[i - 1] == '/')
                {
                    isInsideComment = false;
                    i--;
                    continue;
                }
            }
            if (isInsideGenericsOutsideRoundBrackets > 0)
            {
                if (letter == '<')
                    isInsideGenericsOutsideRoundBrackets--;
                else if (letter == '>')
                    isInsideGenericsOutsideRoundBrackets--;
                continue;
            }
            switch (letter)
            {
                case '=':
                case ' ':
                    if (isInsideRoundBrackets)
                        sb.Insert(0, letter); //Внутри скобок мы все это добавляем
                    else if (sb.Length > 0) // Если мы натыкаемся на нелитерал и мы не в скобках, то это может быть нелитерал, идущий перед литералом-параметром лямбды
                    {
                        result.Add(new Parameter(sb.ToString()));
                        sb.Clear();
                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                    break;
                case '\r':
                    break;
                //Если натыкаемся на точку с запятой значит мы натнулись на предыдущую команду - дальше нам здесь делать нечего.
                case ';':
                    goto end;
                case '>':
                    if (text[i - 1] == '=')
                    {
                        arrowEncountered = true;
                        i--;
                    }
                    else if (isInsideRoundBrackets) //Если мы внутри круглых скобок - дженерики записываем
                    {
                        sb.Insert(0, letter);
                        isInsideGenericsInsideRoundBrackets++;
                    }
                    else
                        isInsideGenericsOutsideRoundBrackets++;
                    break;
                case '<':
                    if (isInsideRoundBrackets) //Если мы внутри круглых скобок - дженерики записываем
                    {
                        sb.Insert(0, letter);
                        isInsideGenericsInsideRoundBrackets--;
                    }
                    break;
                case '/':
                    if (text[i - 1] == '*')
                    {
                        isInsideComment = true;
                        i--;
                    }
                    break;
                case '\n':
                    //Если встретили символ новой строки - проверяем всю эту строку на предмет комментариев, которые можно заигнорировать
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var testLetter = text[j];
                        if (testLetter == '\n')
                            break; //Если прошли всю строку и ничего похожего на однострочный комментарий не обнаружили, от нас ничего не требуется
                        else if (testLetter == '/')
                        {
                            if (text[j - 1] == '/')
                            {
                                i = j - 2; //Если обнаружили однострочный комментарий - игнорируем
                                break;
                            }
                        }
                    }
                    break;
                case ')':
                    //Ок, мы наткнулись на скобки - тут без вариантов начинаются параметры. Еще скобки могут быть в дженериках, но мы этот вариант уже обработали.
                    isInsideRoundBrackets = true;
                    break;
                case '(':
                    if (isInsideRoundBrackets)
                    {
                        if (sb.Length > 0)
                        {
                            result.Add(new Parameter(sb.ToString()));
                            sb.Clear();
                        }
                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                    else //Если мы наткнулись на открывающую скобку, но не наткнулись перед этим на закрывающую - значит мы вышли из нашего scope (допустим мы теперь в скбоках, принимающих лямбду)
                    {
                        if (sb.Length > 0)
                        {
                            result.Add(new Parameter(sb.ToString()));
                            sb.Clear();
                        }
                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                case ',':
                    if (isInsideRoundBrackets)
                    {
                        if (isInsideGenericsInsideRoundBrackets > 0) //Запятые внутри дженериков - нормально
                            sb.Insert(0, letter);
                        else
                        {
                            result.Add(new Parameter(sb.ToString()));
                            sb.Clear();
                        }
                    }
                    break;
                default:
                    //сюда отбираются литералы - их мы читаем только если мы встретили стрелку (один параметр) или круглые скобки (много параметров)
                    if (arrowEncountered || isInsideRoundBrackets)
                        sb.Insert(0, letter);
                    else
                    {
                        sb.Clear();
                        goto end;
                    }
                    break;
            }
        }
        sb.Clear();
    end:
        return result;
    }

    private static int parseMultiLineComment(string text, int i)
    {
        for (i += 2; i < text.Length; i++)
            if (text[i] == '*' && text[++i] == '/')
                return i;
        return -1;
    }

    private static int parseSingleLineComment(string text, int i)
    {
        for (i += 2; i < text.Length; i++)
            if (text[i] == '\n')
                return i;
        return -1;
    }

    private static int parseString(string text, int i)
    {
        var isInterpolated = i > 0 && text[i - 1] == '$';

        //var testString = $"{$"{1}"}";
        if (isInterpolated)
        {
            var curlyBracketsCount = 0;
            for (i++; i < text.Length; i++)
            {
                var letter = text[i];
                if (letter == '{')
                    curlyBracketsCount++;
                else if (letter == '}')
                    curlyBracketsCount--;
                else if (letter == '"' && curlyBracketsCount == 0)
                    return i;
            }
        }
        else
            for (i++; i < text.Length; i++)
                if (text[i] == '"')
                    return i;
        return -1;
    }
}
