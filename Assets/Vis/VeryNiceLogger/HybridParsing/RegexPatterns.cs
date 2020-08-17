using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegexPatterns
{
    public const string AnythingIncludingNewLine = @"(?:[\s\S])";
    public const string WordNotPrecedingByNumber = @"(?:[a-zA-Z_]{1}\w*)";

    public static string MethodRegex() => $"";

    internal static bool IsBlocksContainer(string precedingExpression)
    {

        throw new NotImplementedException();
    }
}
