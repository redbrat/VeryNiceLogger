using System.Collections.Generic;
using UnityEngine;

/*
 * Ок, это работает так, у нас есть дерево логики, из объектов этого класса. Эта структура представляет
 * рекурсивную структуру кода. Текущая выполняемая логика представляет то, что мы сейчас ожидаем увидеть.
 * При этом в одно и то же время мы можем ожидать с одинаковой вероятностью несколько разных вещей. 
 * Например, в одно и то же время мы можем ожидать объявление неймспейса, класса, инструкции using, 
 * whitespace и комментарии. А внутри комментария мы не ожидаем ничего. Соответственно нам нужна структура
 * типа такой:
 * namespace -> все то же самое
 * class ->
 *      Методы ->
 *          локальные переменные
 *          инструкции
 *          локальные методы
 *          Просто блоки кода
 *          Вызовы
 *          whitespace
 *          comments
 *      Проперти
 *      Филды
 *      whitespace
 *      comments
 * usings
 * whitespace
 * comments
 * 
 * Получается, если мы начали проверять какую-то версию, мы должны запомнить на чем мы начали, чтобы если
 * match не удался, мы бы отмотали назад и проверили другую версию. В общем, как регекс только попроще и с 
 * рекурсией.
 * 
 * Так что, что нам нужно - это сначала проверить внутренние условия, и, если есть match, то, если мы 
 * рекурсивны - идем внутрь, и, когда все дети закончились мы ищем закрывающее условие. Если мы не рекурсивны,
 * то просто выходим наверх.
 * 
 * Чтобы не плодить переменные состояния, лучше если собственная логика ноды будет в энумераторе.
 */

public abstract class RecursiveParserBase
{
    protected string _string;
    private RecursiveParserBase[] _children;

    public RecursiveParserBase(string @string, params RecursiveParserBase[] children)
    {
        _string = @string;
        _children = children;
    }
    public RecursiveParserBase(params RecursiveParserBase[] children)
    {
        _children = children;
        for (int i = 0; i < children.Length; i++)
            children[i]._string = _string;
    }

    public RecursiveParserBase()
    {
    }

    public IEnumerator<CommandPositionPair> TryMatch(int position)
    {
        var openTryEnumerator = parseOpen(position);
        var openTryResult = default(CommandPositionPair);
        while (openTryEnumerator.MoveNext())
        {
            openTryResult = openTryEnumerator.Current;
            switch (openTryResult.Command)
            {
                case Commands.Pass:
                    yield return new CommandPositionPair(Commands.Pass);
                    goto ExitCloseParse;
                case Commands.Match:
                    var childrenResult = runChildren();
                    switch (childrenResult.Command)
                    {
                        case Commands.Pass:
                            yield return new CommandPositionPair(Commands.Pass);
                            goto ExitOpenParse;
                        case Commands.Match:
                            position = childrenResult.Position;
                            goto ExitOpenParse;
                    }
                    break;
            }
        }

        CommandPositionPair runChildren()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                var enumerator = _children[i].TryMatch(position);
                while (enumerator.MoveNext())
                {
                    var childResult = enumerator.Current;
                    switch (childResult.Command)
                    {
                        case Commands.Pass:
                            goto NextChildren;
                        case Commands.Match:
                            return childResult;
                    }
                }
            NextChildren:
                {

                }
            }
            return new CommandPositionPair(Commands.Pass);
        }

        ExitOpenParse:
        if (openTryResult.Command == Commands.Pass)
        {
            var closeTryEnumerator = parseOpen(position);
            while (closeTryEnumerator.MoveNext())
            {
                var closeTryResult = closeTryEnumerator.Current;
                switch (closeTryResult.Command)
                {
                    case Commands.Pass:
                        yield return new CommandPositionPair(Commands.Pass);
                        goto ExitCloseParse;
                    case Commands.Match:
                        yield return new CommandPositionPair(position, Commands.Match);
                        goto ExitCloseParse;
                }
            }
        }
        ExitCloseParse:
        {

        }
    }

    protected abstract IEnumerator<CommandPositionPair> parseOpen(int position);
    protected abstract IEnumerator<CommandPositionPair> parseClose(int position);
}
