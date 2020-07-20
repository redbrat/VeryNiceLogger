using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace aajksd
{
    public class aspd
    {

    }
    namespace aajksd21
    {
        public class asdko
        {

        }
    }
}

public static class CodeParser
{
    public static void Parse(string code)
    {
        parseRecursively(code, 0);
    }
    private static void parseRecursively(string recursiveNode, int i)
    {
        for (; i < recursiveNode.Length; i++)
        {
            var currentLetter = recursiveNode[i];
            switch (currentLetter)
            {
                case '/':
                    break;
                default:
                    break;
            }
        }
    }
}


/*
 * Ок, если мы находимся между выражениями мы в любом месте можем натнуться на комментарий. Значит все пустое пространство должно содержать проверки на комментарий.
 */