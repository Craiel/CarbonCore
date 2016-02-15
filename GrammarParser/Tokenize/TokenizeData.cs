namespace CarbonCore.GrammarParser.Tokenize
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.GrammarParser.Contracts;
    using CarbonCore.GrammarParser.Terms;

    internal class TokenizeData<T>
            where T : IToken
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TokenizeData(IGrammar grammar, string sourceData)
        {
            this.Grammar = grammar;
            this.SourceData = sourceData;
            this.Results = new List<T>();

            this.KeyCache = new Dictionary<char, IList<TermKey>>();
            this.IdentifierKeyCache = new Dictionary<string, TermIdentifierKey>();
            if (this.Grammar != null)
            {
                this.BuildCache();
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IGrammar Grammar { get; private set; }

        public IDictionary<char, IList<TermKey>> KeyCache { get; private set; }
        public IDictionary<string, TermIdentifierKey> IdentifierKeyCache { get; private set; }

        public IList<T> Results { get; private set; }

        public char CurrentChar { get; set; }

        public int CurrentOffset { get; set; }
        public int CurrentLineNumber { get; set; }

        public string PendingContent { get; set; }
        public string SourceData { get; private set; }

        public void Finalize(T token)
        {
            this.Results.Add(token);

            int tokenContentDiff = this.PendingContent.Length - token.Contents.Length;
            this.PendingContent = tokenContentDiff > 0 
                ? this.PendingContent.Substring(token.Contents.Length, tokenContentDiff) 
                : string.Empty;
        }

        public Token NewToken(BaseTerm term, string content)
        {
            // Adjust the offset with the content we are putting into this token
            return Token.NewToken(term, this.CurrentLineNumber, this.CurrentOffset - content.Length, content);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void BuildCache()
        {
            this.KeyCache.Clear();

            for (int i = 0; i < this.Grammar.KeyTerms.Count; i++)
            {
                TermKey term = this.Grammar.KeyTerms[i];
                char key = term.Keyword[0];

                IList<TermKey> cache;
                if (this.KeyCache.TryGetValue(key, out cache))
                {
                    this.KeyCache[key].Add(term);
                    continue;
                }

                this.KeyCache.Add(key, new List<TermKey> { term });
            }

            for (int i = 0; i < this.Grammar.IdentifierKeyTerms.Count; i++)
            {
                TermIdentifierKey term = this.Grammar.IdentifierKeyTerms[i];
                if (this.IdentifierKeyCache.ContainsKey(term.Keyword))
                {
                    throw new InvalidOperationException("Duplicate Identifier Key: " + term.Keyword);
                }

                this.IdentifierKeyCache.Add(term.Keyword, term);
            }
        }
    }
}
