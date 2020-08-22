using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public static class RecursiveParserV2
{
    public static CurlyBlock Parse(string text)
    {
        var result = new CurlyBlock(0);
        parseBlockRecursively(text, 0, result);
        return result;
    }

    private static int parseBlockRecursively(string text, int i, CurlyBlock result)
    {
        var info = parseCurlyBlockParameters(text, i - 2);

        result.Info = info;

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
                    var newCurlyBlock = new CurlyBlock(i);
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
        //var isInsideGenericsOutsideRoundBrackets = 0;
        var isInsideGenericsInsideRoundBrackets = 0;
        var isInsideRoundBrackets = false;
        var parametersSb = new StringBuilder();
        var betweenCurlyAndPotentialRoundSb = new StringBuilder();

        var debugLastLetters = default(string);
        var debguLastLettersCount = 36;

        for (; i >= 0; i--)
        {
            var letter = text[i];
            debugLastLetters = text.Substring(Mathf.Max(0, i - debguLastLettersCount), Mathf.Min(debguLastLettersCount, i));

            //if (isInsideGenericsOutsideRoundBrackets > 0)
            //{
            //    if (letter == '<')
            //        isInsideGenericsOutsideRoundBrackets--;
            //    else if (letter == '>')
            //        isInsideGenericsOutsideRoundBrackets--;
            //    continue;
            //}
            switch (letter)
            {
                case '=':
                case ' ':
                    if (isInsideRoundBrackets)
                        parametersSb.Insert(0, letter); //Внутри скобок мы все это добавляем
                    else if (parametersSb.Length > 0) // Если мы натыкаемся на нелитерал, мы не в скобках и мы уже что-то записали, то это может быть только нелитерал, идущий перед литералом-параметром лямбды
                    {
                        parameters.Add(new Parameter(parametersSb.ToString())); //Add, потому что не в скобках может быть только 1 параметр
                        parametersSb.Clear();
                        result.IsLambda = true;
                        result.Parameters = parameters;
                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                    break;
                case ';':
                    //Если натыкаемся на точку с запятой значит мы наткнулись на предыдущую команду - дальше нам здесь делать нечего.
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
                        parametersSb.Insert(0, letter);
                        isInsideGenericsInsideRoundBrackets++;
                    }
                    else //Дженерики у нас могут быть у класса, надо записать их в определение
                        i = parseGenericsReverse(text, i, betweenCurlyAndPotentialRoundSb);
                    break;
                case '<':
                    if (isInsideRoundBrackets) //Если мы внутри круглых скобок - дженерики записываем
                    {
                        parametersSb.Insert(0, letter);
                        isInsideGenericsInsideRoundBrackets--;
                    }
                    break;
                case '/':
                    if (text[i - 1] == '*')
                        i = parseMultiLineCommentReverse(text, i);
                    break;
                case '\r': //\r стараемся игнорировать из-за ее непостоянности
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
                        if (parametersSb.Length > 0)
                        {
                            parameters.Insert(0, new Parameter(parametersSb.ToString()));
                            parametersSb.Clear();
                        }
                        //Если мы видим открывающие скобки, то дальше мы можем вполне увидеть имя метода
                        result.Method = parseMethod(text, i - 1);

                        var genericNarrowing = betweenCurlyAndPotentialRoundSb.ToString().Trim();
                        betweenCurlyAndPotentialRoundSb.Clear();

                        if (genericNarrowing.Length > 0)
                            result.GenericNarrowing = genericNarrowing;

                        result.Method = parseMethod(text, i - 1);

                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                    else //Если мы наткнулись на открывающую скобку, но не наткнулись перед этим на закрывающую - значит мы вышли из нашего scope (допустим мы теперь в скбоках, принимающих лямбду)
                    {
                        if (parametersSb.Length > 0)
                        {
                            parameters.Add(new Parameter(parametersSb.ToString())); //Add, потому что не в скобках может быть только 1 параметр
                            parametersSb.Clear();
                        }
                        goto end; //Параметры закрыты, нам здесь больше делать нечего.
                    }
                case ',':
                    if (isInsideRoundBrackets)
                    {
                        if (isInsideGenericsInsideRoundBrackets > 0) //Запятые внутри дженериков - нормально
                            parametersSb.Insert(0, letter);
                        else
                        {
                            parameters.Insert(0, new Parameter(parametersSb.ToString()));
                            parametersSb.Clear();
                        }
                    }
                    break;
                default:
                    //сюда отбираются литералы - их мы читаем только если мы встретили стрелку (один параметр) или круглые скобки (много параметров)
                    if (arrowEncountered || isInsideRoundBrackets)
                        parametersSb.Insert(0, letter);
                    else
                    { //Этот случай происходит, когда у нас перед блоком стоит определение класса, неймспейса или проперти - сразу идут литералы и надо выходить
                        parametersSb.Clear();

                        var betweenCurlyAndPotentialRoundResult = betweenCurlyAndPotentialRoundSb.ToString().Trim();
                        betweenCurlyAndPotentialRoundSb.Clear();

                        var enumerator = parseCurlyBlockPrefix(text, i);
                        var defaultExpression = default(StringBuilder);
                        while (enumerator.MoveNext())
                        {
                            var (index, keyword, expression) = enumerator.Current;
                            switch (keyword)
                            {
                                case "class":
                                    result.Class = expression.ToString().Trim();
                                    expression.Clear();

                                    if (betweenCurlyAndPotentialRoundResult.Length > 0)
                                        result.GenericNarrowing = betweenCurlyAndPotentialRoundResult;
                                    goto end;
                                case "namespace":
                                    result.Namespace = expression.ToString().Trim();
                                    expression.Clear();
                                    goto end;
                                default:
                                    defaultExpression = expression;
                                    i = index;
                                    break;
                            }
                        }

                        var notAnImmediateClassOrNamespace = $"{defaultExpression.ToString().Trim()}{betweenCurlyAndPotentialRoundResult}";
                        defaultExpression.Clear();

                        if (!notAnImmediateClassOrNamespace.Contains(":"))
                        {
                            //гет и сет, слава богу, не могут содержать всяких дженериков, параметров и т.д. - они всегда либо состоят из get\set либо оканчиваются через пробел или новую строку
                            if (notAnImmediateClassOrNamespace.EndsWith("get") || notAnImmediateClassOrNamespace.EndsWith("set"))
                            {
                                if (notAnImmediateClassOrNamespace.Length == 3)
                                {
                                    switch (notAnImmediateClassOrNamespace)
                                    {
                                        case "get":
                                            result.PropertyGet = notAnImmediateClassOrNamespace;
                                            break;
                                        case "set":
                                            result.PropertySet = notAnImmediateClassOrNamespace;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (notAnImmediateClassOrNamespace[notAnImmediateClassOrNamespace.Length - 4])
                                    {
                                        case '\r':
                                        case '\n':
                                            switch (notAnImmediateClassOrNamespace.Substring(notAnImmediateClassOrNamespace.Length - 3))
                                            {
                                                case "get":
                                                    result.PropertyGet = notAnImmediateClassOrNamespace;
                                                    break;
                                                case "set":
                                                    result.PropertySet = notAnImmediateClassOrNamespace;
                                                    break;
                                            }
                                            break;
                                        default:
                                            result.Property = notAnImmediateClassOrNamespace;
                                            break;
                                    }
                                }
                            }
                            else
                                result.Property = notAnImmediateClassOrNamespace;
                            goto end;
                        }
                        else //а если встретилось :, значит мы прочитали уточнение дженерика функции или класса и надо парсить дальше
                            betweenCurlyAndPotentialRoundSb.Append(notAnImmediateClassOrNamespace);

                        //var classResult = parseClassOrNamespace(text, i, "class");
                        //if (classResult != default)
                        //{
                        //    classResult = classResult.Trim();
                        //    if (classResult.Length > 0)
                        //        result.Class = $"{classResult}{betweenCurlyAndPotentialRoundResult}";
                        //}

                        //var namespaceResult = parseClassOrNamespace(text, i, "namespace");
                        //if (namespaceResult != default)
                        //{
                        //    namespaceResult = namespaceResult.Trim();
                        //    if (namespaceResult.Length > 0)
                        //        result.Namespace = $"{namespaceResult}{betweenCurlyAndPotentialRoundResult}";
                        //}

                    }
                    break;
            }
        }
        parametersSb.Clear();
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
                case ' ':
                    //В имени метода литералы разделяются пробелами или новой строкой, поэтому встретив их это для нас знак что можно проверять наличие имени метода
                    if (sb.Length > 0)
                    {
                        var potentialMethodString = sb.ToString();
                        sb.Clear();
                        return potentialMethodString;
                    }
                    else if (letter == '\n') //Но если имени метода нет а есть перенос строки - проверяем на комменты и идем дальше
                        i = parseSingleLineCommentReverse(text, i);
                    break;
                case '\r': //Игнорируем \r
                    break;
                case '>': //В имени метода могут быть дженерики, поэтому допустимы угловые скобки, пробелы, запятые внутри них и литералы везде, но все остальное ни-ни-ни
                    i = parseGenericsReverse(text, i, sb);
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
                    addToSb.Insert(0, letter);
                    break;
                case '<':
                    genericsLevel--;
                    addToSb.Insert(0, letter);
                    if (genericsLevel == 0)
                        return i;
                    break;
                case '/':
                    if (text[i - 1] == '*')
                        i = parseMultiLineCommentReverse(text, i);
                    break;
                case '\n':
                    addToSb.Insert(0, letter); //Мы должны добавлять знак переноса строки, т.к. это такой же полноправный разделитель всяких ключевых и не очень слов как и пробел
                    i = parseSingleLineCommentReverse(text, i);
                    break;
                case '\r': //а вот возвращение каретки нам без надобности
                    break;
                default:
                    addToSb.Insert(0, letter);
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

    private static IEnumerator<(int index, string word, StringBuilder allTextSb)> parseCurlyBlockPrefix(string text, int i/*, string whatToFind*/)
    {
        var commonSb = new StringBuilder();
        var wordSB = new StringBuilder();

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
                case ' ':
                    //В определении класса или неймспейса литералы разделяются пробелами или новой строкой, поэтому встретив их это для нас знак что можно проверять наличие класса.
                    var wordString = wordSB.ToString();
                    wordSB.Clear();
                    yield return (i, wordString, commonSb); //тут просто i, потому что если там класс или неймспейс - мы все равно сразу выйдем, а если не выйдем - значит функция, а значит мы выйдем не здесь а по ), где уже будет +1
                    //if (wordString == whatToFind)
                    //{
                    //    var classOrNamespaceDefinition = commonSb.ToString();
                    //    commonSb.Clear();
                    //    return classOrNamespaceDefinition;
                    //}
                    //else
                    commonSb.Insert(0, wordString);
                    commonSb.Insert(0, letter);
                    if (letter == '\n') //Если буква - перенос, проверяем на комменты.
                        i = parseSingleLineCommentReverse(text, i);
                    break;
                case '\r': //Игнорируем \r
                    break;
                case '}':
                case ']':
                case ')':
                case '{':
                case '[':
                case '(':
                case ';':
                    goto end; //В определении класса или неймспейса не может быть этих скобок, поэтому увидев их мы сразу идем взад. Также если наткнулись на ; то это уже скорее всего юзинги пошли.
                default:
                    wordSB.Insert(0, letter);
                    break;
            }
        }
    end:
        yield return (i + 1, default, commonSb); // + 1 потому что выйдя из префикса нам еще парсить параметры, а значит - распознать )
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
