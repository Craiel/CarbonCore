namespace CarbonCore.GrammarParser.Grammars
{
    using CarbonCore.GrammarParser;
    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.Terms;
    
    // Based very loosely on https://github.com/KevinHoward/Irony/tree/master/Irony.Samples/Java
    public partial class JavaGrammar : Grammar, IJavaGrammar
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public JavaGrammar()
        {
            var singleLineComment = new TermComment("SingleLineComment", "//", null, "\r", "\n", "\u2085", "\u2028", "\u2029");
            var delimitedComment = new TermComment("DelimitedComment", "/*", null, "*/");
            this.AddTerm(singleLineComment);
            this.AddTerm(delimitedComment);
            
            this.Punctuation = new TermPunctuation(null, ';', ',', '(', ')', '{', '}', '[', ']', ':', '@');

            var validStartCharacters = InitializeCharacterSet(RangeValidIdentStart);
            var validCharacters = InitializeCharacterSet(RangeValidIdent);
            this.Identifier = new TermIdentifier(JavaTermKey.Identifier.ToString(), validStartCharacters, validCharacters, JavaTermKey.Identifier);

            var stringDefault = new TermString(JavaTermKey.String.ToString(), '"', '\\', JavaTermKey.String);
            var charDefault = new TermString(JavaTermKey.Char.ToString(), '\'', '\\', JavaTermKey.Char);
            this.AddTerm(stringDefault);
            this.AddTerm(charDefault);

            var numberTerm = new TermNumber(JavaTermKey.Number.ToString(), tag: JavaTermKey.Number);
            numberTerm.AddExtension("0x", TermNumberTypeCode.Hex);
            numberTerm.AddExtension("l", TermNumberTypeCode.Int64);
            numberTerm.AddExtension("f", TermNumberTypeCode.Float);
            numberTerm.AddExtension("d", TermNumberTypeCode.Double);
            this.Numbers = numberTerm;
            
            this.ToIdentifierTerm("null", JavaTermKey.Null);
            this.ToIdentifierTerm("abstract", JavaTermKey.Abstract);
            this.ToIdentifierTerm("assert", JavaTermKey.Assert);
            this.ToIdentifierTerm("boolean", JavaTermKey.Bool);
            this.ToIdentifierTerm("break", JavaTermKey.Break);
            this.ToIdentifierTerm("byte", JavaTermKey.Byte);
            this.ToIdentifierTerm("case", JavaTermKey.Case);
            this.ToIdentifierTerm("catch", JavaTermKey.Catch);
            this.ToIdentifierTerm("char", JavaTermKey.Char);
            this.ToIdentifierTerm("class", JavaTermKey.Class);
            this.ToIdentifierTerm("const", JavaTermKey.Const);
            this.ToIdentifierTerm("continue", JavaTermKey.Continue);
            this.ToIdentifierTerm("default", JavaTermKey.Default);
            this.ToIdentifierTerm("do", JavaTermKey.Do);
            this.ToIdentifierTerm("double", JavaTermKey.Double);
            this.ToIdentifierTerm("else", JavaTermKey.Else);
            this.ToIdentifierTerm("enum", JavaTermKey.Enum);
            this.ToIdentifierTerm("extends", JavaTermKey.Extends);
            this.ToIdentifierTerm("false", JavaTermKey.False);
            this.ToIdentifierTerm("final", JavaTermKey.Final);
            this.ToIdentifierTerm("finally", JavaTermKey.Finally);
            this.ToIdentifierTerm("float", JavaTermKey.Float);
            this.ToIdentifierTerm("for", JavaTermKey.For);
            this.ToIdentifierTerm("goto", JavaTermKey.Goto);
            this.ToIdentifierTerm("if", JavaTermKey.If);
            this.ToIdentifierTerm("implements", JavaTermKey.Implements);
            this.ToIdentifierTerm("import", JavaTermKey.Import);
            this.ToIdentifierTerm("instanceof", JavaTermKey.InstanceOf);
            this.ToIdentifierTerm("int", JavaTermKey.Int);
            this.ToIdentifierTerm("interface", JavaTermKey.Interface);
            this.ToIdentifierTerm("long", JavaTermKey.Long);
            this.ToIdentifierTerm("native", JavaTermKey.Native);
            this.ToIdentifierTerm("new", JavaTermKey.New);
            this.ToIdentifierTerm("package", JavaTermKey.Package);
            this.ToIdentifierTerm("private", JavaTermKey.Private);
            this.ToIdentifierTerm("protected", JavaTermKey.Protected);
            this.ToIdentifierTerm("public", JavaTermKey.Public);
            this.ToIdentifierTerm("return", JavaTermKey.Return);
            this.ToIdentifierTerm("short", JavaTermKey.Short);
            this.ToIdentifierTerm("static", JavaTermKey.Static);
            this.ToIdentifierTerm("strictfp", JavaTermKey.StrictFP);
            this.ToIdentifierTerm("switch", JavaTermKey.Switch);
            this.ToIdentifierTerm("synchronized", JavaTermKey.Synchronized);
            this.ToIdentifierTerm("throw", JavaTermKey.Throw);
            this.ToIdentifierTerm("throws", JavaTermKey.Throws);
            this.ToIdentifierTerm("transient", JavaTermKey.Transient);
            this.ToIdentifierTerm("super", JavaTermKey.Super);
            this.ToIdentifierTerm("void", JavaTermKey.Void);
            this.ToIdentifierTerm("volatile", JavaTermKey.Volatile);
            this.ToIdentifierTerm("while", JavaTermKey.While);
            this.ToIdentifierTerm("this", JavaTermKey.This);
            this.ToIdentifierTerm("true", JavaTermKey.True);
            this.ToIdentifierTerm("try", JavaTermKey.Try);

            this.ToTerm("&", JavaTermKey.Amp);
            this.ToTerm("&&", JavaTermKey.Amp2);
            this.ToTerm("&=", JavaTermKey.AmpEquals);
            this.ToTerm("=", JavaTermKey.Equal);
            this.ToTerm("@", JavaTermKey.At);
            this.ToTerm("|", JavaTermKey.Pipe);
            this.ToTerm("||", JavaTermKey.Pipe2);
            this.ToTerm("|=", JavaTermKey.PipeEquals);
            this.ToTerm("^", JavaTermKey.Caret);
            this.ToTerm("^=", JavaTermKey.CaretEquals);
            this.ToTerm(":", JavaTermKey.Colon);
            this.ToTerm(",", JavaTermKey.Comma);
            this.ToTerm(".", JavaTermKey.Dot);
            this.ToTerm("...", JavaTermKey.Dot3);
            this.ToTerm("!", JavaTermKey.Exclamation);
            this.ToTerm("==", JavaTermKey.Equals2);
            this.ToTerm(">", JavaTermKey.Greater);
            this.ToTerm(">=", JavaTermKey.GreaterEquals);
            this.ToTerm("<", JavaTermKey.Less);
            this.ToTerm("<=", JavaTermKey.LessEquals);
            this.ToTerm("-", JavaTermKey.Minus);
            this.ToTerm("--", JavaTermKey.Minus2);
            this.ToTerm("-=", JavaTermKey.MinusEquals);
            this.ToTerm("!=", JavaTermKey.ExclamationEquals);
            this.ToTerm("%", JavaTermKey.Percent);
            this.ToTerm("%=", JavaTermKey.PercentEquals);
            this.ToTerm("+", JavaTermKey.Plus);
            this.ToTerm("++", JavaTermKey.Plus2);
            this.ToTerm("+=", JavaTermKey.PlusEquals);
            this.ToTerm("?", JavaTermKey.Question);
            this.ToTerm("[", JavaTermKey.BracketLeft);
            this.ToTerm("]", JavaTermKey.BracketRight);
            this.ToTerm("{", JavaTermKey.BraceLeft);
            this.ToTerm("}", JavaTermKey.BraceRight);
            this.ToTerm("(", JavaTermKey.ParenthesisLeft);
            this.ToTerm(")", JavaTermKey.ParenthesisRight);
            this.ToTerm(";", JavaTermKey.Semicolon);
            this.ToTerm("<<", JavaTermKey.ShiftLeft);
            this.ToTerm("<<=", JavaTermKey.ShiftLeftEquals);
            this.ToTerm(">>", JavaTermKey.ShiftRight);
            this.ToTerm(">>=", JavaTermKey.ShiftRightEquals);
            this.ToTerm("<<<", JavaTermKey.ShiftLeftUnsigned);
            this.ToTerm("<<<=", JavaTermKey.ShiftLeftUnsignedEquals);
            this.ToTerm(">>>", JavaTermKey.ShiftRightUnsigned);
            this.ToTerm(">>>=", JavaTermKey.ShiftRightUnsignedEquals);
            this.ToTerm("/", JavaTermKey.Slash);
            this.ToTerm("/=", JavaTermKey.SlashEquals);
            this.ToTerm("*", JavaTermKey.Star);
            this.ToTerm("*=", JavaTermKey.StarEquals);
            this.ToTerm("~", JavaTermKey.Tilde);
        }
    }
}
