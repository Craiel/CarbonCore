namespace CarbonCore.GrammarParser.Grammars
{
    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.Terms;

    public class SqlGrammar : Grammar, ISqlGrammar
    {
        private static readonly int[][] RangeValidIdentStart = new[]
                                                                   {
                                                                       new[] { 0, 33 }, new[] { 35, 37 },
                                                                       new[] { 39, 44 }, new[] { 46, 60 },
                                                                       new[] { 62, 123 }, new[] { 125, 65510 }
                                                                   };

        private static readonly int[][] RangeValidIdent = new[]
                                                              {
                                                                  new[] { 0, 33 }, new[] { 35, 37 },
                                                                  new[] { 39, 44 }, new[] { 46, 60 },
                                                                  new[] { 62, 123 }, new[] { 125, 65510 }
                                                              };

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SqlGrammar()
        {
            var stringDefault = new TermString(SqlTermKey.String.ToString(), '\'', '\\', SqlTermKey.String);
            this.AddTerm(stringDefault);

            this.Punctuation = new TermPunctuation(null, ',', '(', ')');

            var validStartCharacters = InitializeCharacterSet(RangeValidIdentStart);
            var validCharacters = InitializeCharacterSet(RangeValidIdent);
            this.Identifier = new TermIdentifier(SqlTermKey.Identifier.ToString(), validStartCharacters, validCharacters, SqlTermKey.Identifier);

            this.ToTerm("=", SqlTermKey.Equal);
            this.ToTerm("<", SqlTermKey.LessThan);
            this.ToTerm(">", SqlTermKey.GreaterThan);
            this.ToTerm(",", SqlTermKey.Delimiter);
            this.ToTerm("(", SqlTermKey.BraceLeft);
            this.ToTerm(")", SqlTermKey.BraceRight);

            this.ToTerm("create", SqlTermKey.Create);
            this.ToTerm("insert", SqlTermKey.Insert);
            this.ToTerm("update", SqlTermKey.Update);
            this.ToTerm("delete", SqlTermKey.Delete);
            this.ToTerm("select", SqlTermKey.Select);
            this.ToTerm("drop", SqlTermKey.Drop);
            this.ToTerm("table", SqlTermKey.Table);
            this.ToTerm("into", SqlTermKey.Into);
            this.ToTerm("from", SqlTermKey.From);
            this.ToTerm("set", SqlTermKey.Set);
            this.ToTerm("on", SqlTermKey.On);
            this.ToTerm("as", SqlTermKey.As);
            this.ToTerm("if", SqlTermKey.If);
            this.ToTerm("exists", SqlTermKey.Exists);
            this.ToTerm("inner", SqlTermKey.Inner);
            this.ToTerm("outer", SqlTermKey.Outer);
            this.ToTerm("join", SqlTermKey.Join);
            this.ToTerm("where", SqlTermKey.Where);
            this.ToTerm("order", SqlTermKey.Order);
            this.ToTerm("group", SqlTermKey.Group);
            this.ToTerm("by", SqlTermKey.By);
        }
    }
}
