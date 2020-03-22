﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CSharpRegex
{
    public const string AnythingIncludingNewLine = @"(?:[\s\S])";
    public const string SingleLineComment = @"(?://.*\n)";
    public const string WordNotPrecedingByNumber = @"(?:[a-zA-Z_]{1}\w*)";
    public const string Static = @"(?<Static>\bstatic\b)";
    public const string Abstract = @"(?<Abstract>\babstract\b)";
    public const string Virtual = @"(?<Virtual>\bvirtual\b)";
    public const string Sealed = @"(?<Sealed>\bsealed\b)";
    public const string Override = @"(?<Override>\boverride\b)";
    public const string New = @"(?<New>\bnew\b)";

    public static readonly Func<string, string> CommaSeparatedOneOrMore = expression => $@"{expression}(?:{IgnoredStuff}*,{IgnoredStuff}*{expression})*";
    public static readonly Func<string, string> BreadcrumbsOneOrMore = expression => $@"{expression}(?:{IgnoredStuff}*\.{IgnoredStuff}*{expression})*";
    public static readonly Func<string, string> NotIgnored = expression => $@"(?:(?<!//.*)(?<!/\*{AnythingIncludingNewLine}*?){expression})";
    public static readonly Func<char, char, string, string, string> NestedCharsGroup = (openChar, closeChar, openName, closeName) => $"^[^{openChar}{closeChar}]*?(((?<{openName}>{openChar})[^{openChar}{closeChar}]*?)+?((?<{closeName}-{openName}>{closeChar})[^{openChar}{closeChar}]*?)+?)*(?({openName})(?!))$";
    public static readonly Func<string, string, string, string, string> NestedStringGroup = (openString, closeString, openName, closeName) => $@"^{AnythingIncludingNewLine}*?(((?<{openName}>{openString}){AnythingIncludingNewLine}*?)+?((?<{closeName}-{openName}>{closeString}){AnythingIncludingNewLine}*?)+?)*(?({openName})(?!))$";

    public static string GetNestedCharsGroup(char openChar, char closeChar, string openGroupName, string closeGroupName, string leftGap, string rightGap, string expressionBetween)
    {
        return $@"{leftGap}*?(?:(?:(?<{openGroupName}>{openChar}){expressionBetween})+?(?:(?<{closeGroupName}-{openGroupName}>{closeChar}){rightGap}*?)+?)*(?({openGroupName})(?!))";
    }

    public static readonly string TypeParameter = $@"(?<TypeParameter>\b{WordNotPrecedingByNumber}\b)";
    public static readonly string MultilineComment = $@"(?:/\*{AnythingIncludingNewLine}*?\*/)";
    public static readonly string IgnoredStuff = $@"(?:\s|{SingleLineComment}|{MultilineComment})";
    //public static readonly string Generics = $@"(?<Generics>\<{IgnoredStuff}*{CommaSeparatedOneOrMore(TypeParameter)}{IgnoredStuff}*\>)";
    public static readonly string Type = $@"(?<Type>\b{BreadcrumbsOneOrMore(WordNotPrecedingByNumber)}\b)";
    public static readonly string Generics = $@"(?<Generics>{GetNestedCharsGroup('<', '>', "GenericsOpen", "GenericsClose", IgnoredStuff, IgnoredStuff, $"{IgnoredStuff}*{CommaSeparatedOneOrMore(Type)}{IgnoredStuff}*")})";
    public static readonly string GenericType = $@"(?<GenericType>{Type}(?:{IgnoredStuff}*{Generics})?)";
    public static readonly string AccessModifier = $@"(?<AccessModifier>\b(?:public|private(?:{IgnoredStuff}+protected)?|protected(?:{IgnoredStuff}+internal)?|internal)\b)";
    public static readonly string GenericConstrains = $@"(?<GenericConstrains>\bwhere{IgnoredStuff}+{TypeParameter}{IgnoredStuff}*:{IgnoredStuff}*{CommaSeparatedOneOrMore(GenericType)})";
    public static readonly string GenericsConstrains = $@"(?<GenericsConstrains>(?:{GenericConstrains}{IgnoredStuff}*)+)";
    public static readonly string Parameters = $@"";
    public static readonly string NamespaceDeclaration = $@"(?:\bnamespace{IgnoredStuff}+(?<Namespace>{WordNotPrecedingByNumber}))";
    public static readonly string ClassDeclaration = $@"(?:{AccessModifier}{IgnoredStuff}+)?(?:(?:{Static}|{Abstract}|{Sealed}){IgnoredStuff}+)?\bclass{IgnoredStuff}+(?<ClassName>{GenericType})(?<Extending>{IgnoredStuff}*:{IgnoredStuff}*{CommaSeparatedOneOrMore(GenericType)})?{IgnoredStuff}*{GenericsConstrains}?";
    public static readonly string FunctionDeclaration = $@"(?:(?:{AccessModifier}{IgnoredStuff}+)?{New}?(?:{Static}|{Abstract}|{Virtual}{Override})?{Sealed}?(?<ReturnType>{GenericType}){IgnoredStuff}(?<FunctionName>{WordNotPrecedingByNumber}){IgnoredStuff}+{Parameters})";



    public static readonly string RecursiveClasses = $@"{NestedStringGroup($"{ClassDeclaration}{IgnoredStuff}*{{", $"}}", "Open", "Close")}";
}

namespace as2k
{
    public abstract class q923ue9
    {
        public class MeaUapsdo1 { }
        public class MeaUapsdo2: ICloneable
        {
            public object Clone()
            {
                throw new NotImplementedException();
            }
        }

        public abstract string getMyBeautifulString();
        public abstract string getMyUglyString<T>() where T : Type;
    }
    public class ASKDO:q923ue9 , IEnumerable<System.Collections.Generic.Dictionary<string, int>>
    {
        public string getMyGoodString<T, MeaUapsdo1> /*32323*/ () where T : Type where MeaUapsdo1 : System. Collections .Generic . IEnumerable<T>
        {
            return "ASdpojasioas  p[ p[sd pdj";
        }

        public override string getMyBeautifulString()
        {
            throw new NotImplementedException();
        }

        public override string getMyUglyString<T>()
        {
            throw new NotImplementedException();
        }

        protected virtual float getEQWEWE()
        {
            return 1;
        }

        public IEnumerator<Dictionary<string, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class sssss<Captain> :ASKDO ,ICloneable where Captain : ScriptableObject
    {
        public new string getMyBeautifulString()
        {
            return "Bla-bla-bla";
        }
        protected override sealed float getEQWEWE()
        {
            return base.getEQWEWE() + 1;
        }

        public void HailThe(Captain currentCaptain) { }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class ppppp<T, T2, MyMumma> : ASKDO , IEnumerable<T2>where T:UnityEngine.Object , ICloneable
        where T2: Type /*2d2d2*/
        , IComparable,    ILogHandler
        
        ,IDisposable
        where MyMumma :Behaviour
    {
        public new virtual string getMyBeautifulString()
        {
            return "Bla-bla-bla";
        }
        public new virtual string getEQWEWE()
        {
            return "Bla-bla-bla";
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T2> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class asodk4
    { //1112221
        internal static class dsare6
        {
            public static int Twifaso = (int)-123.8f;
        }

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
