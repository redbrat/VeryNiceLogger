using System;

static class CSharpRegex
{
    public const string SingleLineComment = @"(?://.*\n)";
    public const string MultilineComment = @"(?:/\*[\s\S]*?\*/)";
    public static readonly string IgnoredStuff = $@"(?:\s|{SingleLineComment}|{MultilineComment})";
    public static readonly string AccessModifier = $@"(?<AccessModifier>\b(?:public|private(?:{IgnoredStuff}+protected)?|protected(?:{IgnoredStuff}+internal)?|internal)\b)";
    public static readonly string Static = $@"(?<Static>\bstatic\b)";
    public static readonly string Class = $@"(?:{AccessModifier}{IgnoredStuff}+)?(?:{Static}{IgnoredStuff}+)?\bclass{IgnoredStuff}+(?<ClassName>[a-zA-Z_]{{1}}\w*)";

    public static readonly Func<string, string> NotIgnored = expression => $@"(?:(?<!//.*)(?<!/\*[\s\S]*?){expression})";
    public static readonly Func<char, char, string> NestedCharsGroup = (open, close) => $"^[^{open}{close}]*?(((?<Open>{open})[^{open}{close}]*?)+?((?<Close-Open>{close})[^{open}{close}]*?)+?)*(?(Open)(?!))$";
    public static readonly Func<string, string, string> NestedStringGroup = (open, close) => $@"^[\s\S]*?(((?<Open>{open})[\s\S]*?)+?((?<Close-Open>{close})[\s\S]*?)+?)*(?(Open)(?!))$";
}

namespace ask
{
    class asodk
    {
        protected /*asd
            asdoko
            222   
        */
            //qwe
            internal void DoSomething()
        {

        }
    }
}
