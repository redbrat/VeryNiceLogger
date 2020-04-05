using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static class CSharpRegex
{
    /*
     * Мда, не вижу как регексы могут обрабатывает в разумное время сложные рекурсивные структуры. По крайней мере с балансирующими группами не вижу как. 
     * Я уж не говорю о полноценном распознавании рекурсии и то, что внутри нее. Об этом речи нет. Даже тупо чтобы правильно ограничить рекурсию по краям
     * требуется продумать кучу разных возможных сценариев типа, а что если встретится строка? А что если строка будет интерполированная а внутри - лямбда?
     * Короче это очень похоже на затыкание круглой пробкой квадратного отверстия. Бросаю это дело. Было интересно.
     */
    public const string Value = @"(?<Value>\b\w*\b)";
    public const string AnythingIncludingNewLine = @"(?:[\s\S])";
    public const string SingleLineComment = @"(?://.*\n)";
    public const string WordNotPrecedingByNumber = @"(?:[a-zA-Z_]{1}\w*)";
    public const string Static = @"(?<Static>\bstatic\b)";
    public const string Abstract = @"(?<Abstract>\babstract\b)";
    public const string Virtual = @"(?<Virtual>\bvirtual\b)";
    public const string Sealed = @"(?<Sealed>\bsealed\b)";
    public const string Override = @"(?<Override>\boverride\b)";
    public const string New = @"(?<New>\bnew\b)";

    public static string GetCommaSeparatedOneOrMore(string expression, bool greedy = true) => $@"(?:{expression}(?:{IgnoredStuff}*,{IgnoredStuff}*{expression})*{(greedy ? string.Empty : "?")})";

    public static readonly Func<string, string> SeparatedOneOrMore = expression => $@"(?:{expression}(?:{IgnoredStuff}*{expression})*)";
    //public static readonly Func<string, string> CommaSeparatedOneOrMore = expression => $@"(?:{expression}(?:{IgnoredStuff}*,{IgnoredStuff}*{expression})*)";
    public static readonly Func<string, string> DotSeparatedOneOrMore = expression => $@"(?:{expression}(?:{IgnoredStuff}*\.{IgnoredStuff}*{expression})*)";
    public static readonly Func<string, string> NotIgnored = expression => $@"(?:(?<!//.*)(?<!/\*{AnythingIncludingNewLine}*?){expression})";

    //public static string GetNestedCharsGroup(char openChar, char closeChar, string openGroupName, string closeGroupName, string leftGap, string rightGap, string expressionBetween) => $@"(?:{leftGap}*?(?:(?:(?<{openGroupName}>{openChar}){expressionBetween})+?(?:(?<{closeGroupName}-{openGroupName}>{closeChar}){rightGap}*?)+?)+(?(?:{openGroupName})(?!)))";
    public static string GetNestedCharsGroup(char openChar, char closeChar, string openGroupName, string closeGroupName) => $@"\{openChar}(?>\{openChar}(?<{openGroupName}>)|[^{openChar}{closeChar}]+|\{closeChar}(?<{closeGroupName}-{openGroupName}>))*(?({openGroupName})(?!))\{closeChar}";
    public static string GetNestedStringsGroup(string openString, string closeString, string openGroupName, string closeGroupName, string leftGap, string rightGap, string expressionBetween) => $@"(?:{leftGap}*?(?:(?:(?<{openGroupName}>{openString}){expressionBetween})+?(?:(?<{closeGroupName}-{openGroupName}>{closeString}){rightGap}*?)+?)*(?(?:{openGroupName})(?!)))";

    public static readonly string TypeParameter = $@"(?<TypeParameter>\b{WordNotPrecedingByNumber}\b)";
    public static readonly string MultilineComment = $@"(?:/\*{AnythingIncludingNewLine}*?\*/)";
    public static readonly string IgnoredStuff = $@"(?:\s|{SingleLineComment}|{MultilineComment})";
    //public static readonly string Generics = $@"(?<Generics>\<{IgnoredStuff}*{CommaSeparatedOneOrMore(TypeParameter)}{IgnoredStuff}*\>)";
    public static readonly string Type = $@"(?<Type>\b{DotSeparatedOneOrMore(WordNotPrecedingByNumber)}\b)";
    public static readonly string Generics = $@"(?<Generics>{GetNestedCharsGroup('<', '>', "GenericsOpen", "GenericsClose")})";
    //public static readonly string Generics = $@"(?<Generics>{GetNestedStringsGroup('<'.ToString(), '>'.ToString(), "GenericsOpen", "GenericsClose", IgnoredStuff, IgnoredStuff, $"{IgnoredStuff}*{CommaSeparatedOneOrMore(Type)}?{IgnoredStuff}*")})";
    public static readonly string GenericType = $@"(?<GenericType>{Type}(?:{IgnoredStuff}*{Generics})?)";
    public static readonly string ArrayGenericType = $@"(?<ArrayGenericType>{GenericType}(?<IsArray>{IgnoredStuff}*?\[{IgnoredStuff}*?\])?)";
    public static readonly string AccessModifier = $@"(?<AccessModifier>\b(?:public|private(?:{IgnoredStuff}+protected)?|protected(?:{IgnoredStuff}+internal)?|internal)\b)";
    public static readonly string GenericConstrains = $@"(?<GenericConstrains>\bwhere{IgnoredStuff}+{TypeParameter}{IgnoredStuff}*:{IgnoredStuff}*{GetCommaSeparatedOneOrMore(GenericType)})";
    public static readonly string GenericsConstrains = $@"(?<GenericsConstrains>{SeparatedOneOrMore(GenericConstrains)})";
    public static readonly string NamespaceDeclaration = $@"(?<NamespaceDeclaration>\bnamespace{IgnoredStuff}+(?<Namespace>{WordNotPrecedingByNumber}))";
    public static readonly string ClassDeclaration = $@"(?<ClassDeclaration>{AccessModifier}{IgnoredStuff}+)?(?:(?:{Static}|{Abstract}|{Sealed}){IgnoredStuff}+)?\bclass{IgnoredStuff}+(?<ClassName>{GenericType})(?<Extending>{IgnoredStuff}*:{IgnoredStuff}*{GetCommaSeparatedOneOrMore(GenericType)})?{IgnoredStuff}*{GenericsConstrains}?";

    public static readonly string Variable = $@"(?<Variable>\b{WordNotPrecedingByNumber}\b)";
    public static readonly string GenericTypedVariable = $@"(?<GenericTypedVariable>{GenericType}{IgnoredStuff}*{Variable}(?<GenericTypedVariableDefaultValue>{IgnoredStuff}*={IgnoredStuff}*{Value})?)";
    public static readonly string Parameters = $@"(?<Parameters>\({GetCommaSeparatedOneOrMore(GenericTypedVariable, false)}??\))";
    public static readonly string FunctionDeclaration = $@"(?<FunctionDeclaration>(?:{AccessModifier}{IgnoredStuff}+?)??{New}??(?:(?:{Static}|{Abstract}|{Virtual}|{Override}){IgnoredStuff}*?)??(?:{Sealed}{IgnoredStuff}*?)??(?<ReturnType>{ArrayGenericType}){IgnoredStuff}*?(?<FunctionName>\b{WordNotPrecedingByNumber}\b)(?:{IgnoredStuff}*?{Generics})??{IgnoredStuff}*?{Parameters}{IgnoredStuff}*?{GenericsConstrains}??{IgnoredStuff}*?(?={{))";


    public static readonly string Body = $@"(?<Body>(?<ExpressionBody>(?:=\>{IgnoredStuff}*)[^;]*;)|(?<FullBody>{GetNestedCharsGroup('{', '}', "OpenCurlyBracket", "CloseCurlyBracket")}))";
    public static readonly string GetProperty = $@"(?<GetProperty>(?:{AccessModifier}{IgnoredStuff}+?)??\bget\b{IgnoredStuff}*{Body})";
    public static readonly string SetProperty = $@"(?<SetProperty>(?:{AccessModifier}{IgnoredStuff}+?)??\bset\b{IgnoredStuff}*{Body})";
    public static readonly string GetOrSetProperty = $@"(?:{GetProperty}|{SetProperty})";
    public static readonly string PropertyDeclaration = $@"(?<PropertyDeclaration>(?:{AccessModifier}{IgnoredStuff}*?)??(?:{New}{IgnoredStuff}*?)??(?:(?:{Static}|{Abstract}|{Virtual}|{Override}){IgnoredStuff}*?)??(?:{Sealed}{IgnoredStuff}*?)??(?<TypeOfProperty>{GenericType}){IgnoredStuff}*?{{(?:{IgnoredStuff}*?{GetOrSetProperty}{IgnoredStuff}*?)*?}})";

    //public static readonly string RecursiveClasses = $@"^{GetNestedStringsGroup($"{{", $"}}", "ClassOpen", "ClassClose", AnythingIncludingNewLine, AnythingIncludingNewLine, $"(?:{AnythingIncludingNewLine})*")}$";
    public static readonly string RecursiveClasses = $@"^{GetNestedStringsGroup($"(?:{ClassDeclaration}{{)", $"}}", "ClassOpen", "ClassClose", AnythingIncludingNewLine, AnythingIncludingNewLine, $"(?:{AnythingIncludingNewLine})*")}$";
    //public static readonly string RecursiveClasses = $@"{GetNestedStringsGroup($"{ClassDeclaration}{{", $"}}", "ClassOpen", "ClassClose", IgnoredStuff, IgnoredStuff, AnythingIncludingNewLine)}";

    public static readonly string LambdaBody = $@"(?<LambdaBody>(?<ExpressionBody>(?:=>{IgnoredStuff}*)[^;{{}}]*;)|(?<FullBody>(?:=>{IgnoredStuff}*){GetNestedCharsGroup('{', '}', "OpenCurlyBracket", "CloseCurlyBracket")}))";
    public static readonly string LambdaParameters = $@"(?<LambdaParameters>(?<ScopedLambdaParameters>\((?:(?<TypedLambdaParameters>{GetCommaSeparatedOneOrMore(GenericTypedVariable, false)}??)|(?<NonTypedLambdaParameters>{GetCommaSeparatedOneOrMore(Variable, false)}??))\))|(?<UnscopedLambdaParameters>{Variable}))";
    public static readonly string Lambda = $@"(?<Lambda>{LambdaParameters}{IgnoredStuff}*{LambdaBody})";
}

namespace as2k
{
    public abstract class q923ue9
    {
        public int [ /*asd*/ ] asdkok;
        public abstract object AbstractProp { get; set; }
        public class MeaUapsdo1 { }
        public class MeaUapsdo2 : ICloneable
        {
            public object Clone()
            {
                var a = (Action)(() => Debug.Log("asdlp"));

                Action<int> b = _ => { };

                Func<int, int> c = ASodjkapoDJSDOPA => ASodjkapoDJSDOPA;
                Func<int, int> c0 = (ASodjkapoDJSDOPA) => ASodjkapoDJSDOPA;
                Func<int, int> c00 = (int ASodjkapoDJSDOPA) => ASodjkapoDJSDOPA;
                Func<int, int> c000 = (int ASodjkapoDJSDOPA) => ASodjkapoDJSDOPA;

                var a2 = (Action)(() => Debug.Log("asdlp"));

                var d = (Action)(() => { Debug.Log("asdlp"); });

                Func<int, int> e = ASodjkapoDJSDOPA => { return ASodjkapoDJSDOPA; };

                var f = (Action)(() =>
                {
                    Debug.Log("asdlapd");

                    var a1 = 2 + 5;
                    Console.WriteLine("2o0do0");
                    {

                        int b1 = a1 + 11;
                    }
                });

                Action<int> g = _ =>
                {
                    string aw = " aw aw aw !";
                    string askdop = $"{aw}!! {d}, 1";

                    throw new NotImplementedException();

                    Debug.LogError($"aw = {aw}, askdop = {askdop}");
                };

                Func<int, int> h = ASodjkapoDJSDOPA =>
                {
                    var t = Mathf.FloatToHalf(2.3f);
                    return (int)Mathf.Clamp(t + 15, 0, float.MaxValue);
                };

                var i = (Action)(() => throw new NotImplementedException());

                var list = new List<int>();
                var abc = list.Where(i1 => i1 > 0).Select(i_2 => i_2.ToString()).ToDictionary(s => int.Parse(s));

                var asdo = (Func<string, int, object>)((s12, i23) =>
                {
                    var s12i = int.Parse(s12);
                    return i23 + s12i;
                });
                var asdo0 = (Func<string, int, object>)((string s12, int i23) =>
                {
                    var s12i = int.Parse(s12);
                    return i23 + s12i;
                });

                throw new NotImplementedException();
            }
        }

        public abstract string getMyBeautifulString();
        public abstract string getMyUglyString<T>() where T : Type;
    }
    public class ASKDO : q923ue9, IEnumerable<System.Collections.Generic.Dictionary<string, int>>
    {
        public string MyComplexString
        {
            get => $"ajsidj:{_myComplexString}!";
            set => _myComplexString = $"asdopk!{value}:";
        }
        private string _myComplexString;

        public virtual float InterestingProp
        {
            internal set
            {
                _interestingProp = value;
            }
            get => _interestingProp;
        }
        public virtual float InterestingProp2
        {
            private set
            {
                _interestingProp = value;
            }
            get => _interestingProp;
        }
        protected float _interestingProp;
        public override object AbstractProp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual object VirtualProp { get; set; }
        public object Prop1 { get; }
        public Dictionary<List<string>, float> Prop2 { get; private set; }
        public List<int> Prop3 { get; internal set; }
        public object Prop4 { get; protected set; }
        public static int asdjkkoasdj
        {
            get => 1;
            set { }
        }
        public int MyPropertyMYYY1
        {
            get => _myProperty;
            set => _myProperty = value;
        }
        public int MyPropertyMYYY
        {
            get => _myProperty;
            set
            {
                _myProperty = value;
            }
        }
        public int MyPropertyMYYY2
        {
            get
            {
                return _myProperty;
            }
            set => _myProperty = value;
        }
        public int MyPropertyMYYY3
        {
            get
            {
                return _myProperty;
            }
            set
            {
                _myProperty = value;
            }
        }
        public int MyPropertyMYYY4
        {
            get { return _myProperty; }
            set { _myProperty = value; }
        }
        public int MyPropertyMYYY5 { get { return _myProperty; } set { _myProperty = value; } }
        public int MyPropertyMYYY6
        {
            set
            {
                _myProperty = value;
            }
        }
        public int MyPropertyMYYY7
        {
            get
            {
                return _myProperty;
            }
        }
        public int MyPropertyMYYY8
        {
            get
            {
                return _myProperty;
            }
            internal set
            {
                _myProperty = value;
            }
        }
        public int MyPropertyMYYY9
        {
            protected get
            {
                return _myProperty;
            }
            set
            {
                _myProperty = value;
            }
        }
        public int MyPropertyMYYY10
        {
            get
            {
                return _myProperty;
            }
            private set
            {
                _myProperty = value;
            }
        }
        public int MyPropertyMYYY11
        {
            get
            {
                return _myProperty;
            }
            protected internal set
            {
                _myProperty = value;
            }
        }

        private int _myProperty;

        public string getMyGoodString<T, MeaUapsdo1> /*32323*/ () where T : Type where MeaUapsdo1 : System.Collections.Generic.IEnumerable<T>
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

    internal sealed class sssss<Captain> : ASKDO, ICloneable where Captain : ScriptableObject
    {
        public override float InterestingProp { get => base.InterestingProp; internal set => base.InterestingProp = value; }
        public override float InterestingProp2 => base.InterestingProp2;

        public new string getMyBeautifulString()
        {
            return "Bla-bla-bla";
        }
        protected override sealed float getEQWEWE()
        {
            return base.getEQWEWE() + 1;
        }

        public void HailThe(Captain currentCaptain) { }

        public void HailThe(Captain currentCaptain, int tiMes = 13) { }

        public void HailThe(Captain currentCaptain, int[] newYourTimes) { }
        public void HailThe(Captain currentCaptain, Dictionary<int, IEnumerable<Captain>> newYourTimes = null) { }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class ppppp<T, T2, MyMumma> : ASKDO, IEnumerable<T2> where T : UnityEngine.Object, ICloneable
        where T2 : Type /*2d2d2*/
        , IComparable, ILogHandler

        , IDisposable
        where MyMumma : Behaviour
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
