using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public static class RecursiveParserV2
{
    public static CurlyBlock Parse(string text)
    {
        var result = new CurlyBlock();
        parseBlockRecursively(text, 0, result);
        return result;
    }

    private static int parseBlockRecursively(string text, int i, CurlyBlock result)
    {
        var info = parseCurlyBlockParameters(text, i - 2);

        result.Info = info;
        result.StartIndex = i;

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
                    var newCurlyBlock = new CurlyBlock();
                    result.AppendChildBlock(newCurlyBlock);
                    i = parseBlockRecursively(text, i + 1, newCurlyBlock);
                    break;
                case '}':
                    result.EndIndex = i;
                    return i;
                case ';':
                    result.AppendCommand(i);
                    break;

                default:
                    break;
            }
        }
        result.EndIndex = text.Length - 1;
        return -1;
    }

    private class asdjij<isoko> where isoko : ICloneable
    {
        void asiodji<wodoi>() { }
        void asidjio<T>() where T : IEnumerable<isoko>
        {
            Action<int> a = b => { b++; b--; };
        }

        /*
         * Могут ли нам встретиться угловые скобки за фигурными но перед круглыми? Круглые обязательно означают
         * функцию или лямбду, поэтому должна быть функция. Да, может встречаться в уточнении дженерика хотя бы.
         * А раз все возможно в принципе везде, то лучше наверное просто рассчитывать на опознание ключевого 
         * слова. Если найдено class или namespace или ничего не найдено - тогда это функция. Лямбды мы можем 
         * точно определить до этого с помощью =>. Перед фигурной и до => не может быть угловых скобок никак.
         * 
         * Таким образом мы должны записывать все кроме скобок (не угловых). И как только встретили слово class,
         * namespace, или один из модификаторов доступа - делать выводы о том, что перед нами.
         */
    }

    private static CurlyBlockInfo parseCurlyBlockParameters(string text, int i)
    {
        var result = new CurlyBlockInfo();
        var parameters = new List<Parameter>();

        //Action a = () => { };
        //Action<int> b = c => { };
        //Action<int, string> c = (d, e) =>
        ////asdasd
        ///*
        // * sadsad
        // */
        //{ };

        var arrowEncountered = false;
        var isInsideGenericsOutsideRoundBrackets = 0;
        var isInsideGenericsInsideRoundBrackets = 0;
        var isInsideRoundBrackets = false;
        var sb = new StringBuilder();

        var debugLastLetters = default(string);
        var debguLastLettersCount = 36;

        for (; i >= 0; i--)
        {
            var letter = text[i];
            debugLastLetters = text.Substring(Mathf.Max(0, i - debguLastLettersCount), Mathf.Min(debguLastLettersCount, i));

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
                    else if (sb.Length > 0) // Если мы натыкаемся на нелитерал, мы не в скобках и мы уже что-то записали, то это может быть только нелитерал, идущий перед литералом-параметром лямбды
                    {
                        parameters.Add(new Parameter(sb.ToString()));
                        sb.Clear();
                        result.IsLambda = true;
                        result.Parameters = parameters;
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
                        result.IsLambda = true; //Стрелочка может быть и у нормальной функции, но у нормальной функции не может быть одновременно стрелочек и фигурных скобок, поэтому в данном случае стрелочка - всегда лямбда
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
                        i = parseMultiLineCommentReverse(text, i);
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
                    result.Parameters = parameters;
                    break;
                case '(':
                    if (isInsideRoundBrackets)
                    {
                        if (sb.Length > 0)
                        {
                            parameters.Add(new Parameter(sb.ToString()));
                            sb.Clear();
                        }
                        //Если мы видим открывающие скобки, то дальше мы можем вполне увидеть имя метода
                        result.Method = parseMethod(text, i - 1);
                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                    else //Если мы наткнулись на открывающую скобку, но не наткнулись перед этим на закрывающую - значит мы вышли из нашего scope (допустим мы теперь в скбоках, принимающих лямбду)
                    {
                        if (sb.Length > 0)
                        {
                            parameters.Add(new Parameter(sb.ToString()));
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
                            parameters.Add(new Parameter(sb.ToString()));
                            sb.Clear();
                        }
                    }
                    break;
                default:
                    //сюда отбираются литералы - их мы читаем только если мы встретили стрелку (один параметр) или круглые скобки (много параметров)
                    if (arrowEncountered || isInsideRoundBrackets)
                        sb.Insert(0, letter);
                    else
                    { //Этот случай происходит, когда у нас перед блоком стоит определение класса или неймспейса - сразу идут литералы и надо выходить
                        sb.Clear();
                        result.Class = parseClassOrNamespace(text, i, "class");
                        result.Namespace = parseClassOrNamespace(text, i, "namespace");
                        goto end;
                    }
                    break;
            }
        }
        sb.Clear();
    end:
        return result;
    }

    private static string parseMethod(string text, int i)
    {
        var sb = new StringBuilder();
        for (; i >= 0; i--)
        {
            var letter = text[i];

            switch (letter)
            {
                case '/':
                    if (text[i - 1] == '*')
                        i = parseMultiLineCommentReverse(text, i);
                    break;
                case '\n':
                    i = parseSingleLineCommentReverse(text, i);
                    break;
                //В имени метода могут быть дженерики, поэтому допустимы угловые скобки, пробелы, запятые внутри них и литералы везде, но все остальное ни-ни-ни
                case '>':
                    i = parseGenericsReverse(text, i, sb);
                    break;
                case '\r':
                case ' ':
                    //В имени метода литералы разделяются пробелами или новой строкой (\n уже занят, поэтому берем \r - он тоже всегда встречается), поэтому встретив пробел это для нас знак что можно проверять наличие имени метода
                    if (sb.Length > 0)
                    {
                        var potentialMethodString = sb.ToString();
                        sb.Clear();
                        return potentialMethodString;
                    }
                    break;
                case '}':
                case ']':
                case ')':
                case '{':
                case '[':
                case '(':
                case ';':
                    return default; //Видим любую такую скобку - это уже не метод
                default:
                    sb.Insert(0, letter);
                    break;
            }
        }
        return default;
    }

    private static int parseGenericsReverse(string text, int i, StringBuilder addToSb)
    {
        var genericsLevel = 0;

        for (; i >= 0; i--)
        {
            var letter = text[i];

            switch (letter)
            {
                case '>':
                    genericsLevel++;
                    addToSb.Append(letter);
                    break;
                case '<':
                    genericsLevel--;
                    addToSb.Append(letter);
                    if (genericsLevel == 0)
                        return i;
                    break;
                case '/':
                    if (text[i - 1] == '*')
                        i = parseMultiLineCommentReverse(text, i);
                    break;
                case '\n':
                    i = parseSingleLineCommentReverse(text, i);
                    break;
                case '\r': //Переносы строки, как и возвращение каретки нам без надобности
                    break;
                default:
                    addToSb.Append(letter);
                    break;
            }
        }
        return -1;
    }

    private static int parseSingleLineCommentReverse(string text, int i)
    {
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
                    return j - 1; //Если обнаружили однострочный комментарий - игнорируем
                }
            }
        }
        return i;
    }

    private static int parseMultiLineCommentReverse(string text, int i)
    {
        var isInsideComment = false;
        for (; i >= 0; i--)
        {
            var letter = text[i];

            if (isInsideComment)
            {
                if (letter == '*' && text[i - 1] == '/')
                    return i - 1;
                continue;
            }

            if (letter == '/' && text[i - 1] == '*')
            {
                isInsideComment = true;
                i--;
            }
        }
        return -1;
    }

    private static string parseClassOrNamespace(string text, int i, string whatToFind)
    {
        var commonSb = new StringBuilder();
        var classOrNamespaceSB = new StringBuilder();

        for (; i >= 0; i--)
        {
            var letter = text[i];

            switch (letter)
            {
                case '/':
                    if (text[i - 1] == '*')
                        i = parseMultiLineCommentReverse(text, i);
                    break;
                case '\n':
                    i = parseSingleLineCommentReverse(text, i);
                    break;
                case '\r':
                case ' ':
                    //В определении класса или неймспейса литералы разделяются пробелами или новой строкой (\n уже занят, поэтому берем \r - он тоже всегда встречается), поэтому встретив пробел это для нас знак что можно проверять наличие класса
                    var potentialClassString = classOrNamespaceSB.ToString();
                    classOrNamespaceSB.Clear();
                    if (potentialClassString == whatToFind)
                    {
                        var classOrNamespaceDefinition = commonSb.ToString();
                        commonSb.Clear();
                        return classOrNamespaceDefinition;
                    }
                    else
                        commonSb.Insert(0, potentialClassString);
                    break;
                case '}':
                case ']':
                case ')':
                case '{':
                case '[':
                case '(':
                case ';':
                    return default; //В определении класса или неймспейса не может быть этих скобок, поэтому увидев их мы сразу идем взад. Также если наткнулись на ; то это уже скорее всего юзинги пошли.
                default:
                    classOrNamespaceSB.Insert(0, letter);
                    break;
            }
        }
        return default;
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
