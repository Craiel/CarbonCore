namespace CarbonCore.GrammarParser.Tokenize
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using CarbonCore.GrammarParser.Contracts;
    using CarbonCore.GrammarParser.Terms;

    using NLog;

    public class Tokenizer : ITokenizer<Token>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Tokenizer()
        {
            this.IsCaseSensitive = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsCaseSensitive { get; set; }

        public IList<Token> Tokenize(IGrammar grammar, StreamReader reader)
        {
            string data = reader.ReadToEnd();
            return this.Tokenize(grammar, data);
        }

        public IList<Token> Tokenize(IGrammar grammar, string data)
        {
            var tokenData = new TokenizeData<Token>(grammar, data);

            while (this.TokenizeNext(tokenData))
            {
                // TODO: hook for progress display == tokenData.CurrentOffset / data.Length
            }

            return tokenData.Results;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool TokenizeNext(TokenizeData<Token> data)
        {
            while (!string.IsNullOrEmpty(data.PendingContent))
            {
                if (data.Grammar != null && this.FormatToken(data))
                {
                    continue;
                }

                // Add a null token with all pending content
                //  this element is probably not handled by the grammar or we have no grammar at all
                data.Finalize(data.NewToken(null, data.PendingContent));
                return true;
            }

            while (data.CurrentOffset < data.SourceData.Length)
            {
                data.CurrentChar = data.SourceData[data.CurrentOffset++];

                // First step
                switch (data.CurrentChar)
                {
                    case '\n':
                        {
                            data.CurrentLineNumber++;
                            break;
                        }
                }

                // Second step
                switch (data.CurrentChar)
                {
                    case ' ':
                    case '\r':
                    case '\n':
                        {
                            // Rewind and return to have the tokens processed, if we have content
                            if (!string.IsNullOrEmpty(data.PendingContent))
                            {
                                data.CurrentOffset--;
                                return true;
                            }

                            break;
                        }

                    default:
                        {
                            if (this.CheckPunctuation(data))
                            {
                                // Rewind the punctuation if we have data, to get it as a stand alone entry
                                if (!string.IsNullOrEmpty(data.PendingContent))
                                {
                                    data.CurrentOffset--;
                                    return true;
                                }
                            }

                            data.PendingContent += data.CurrentChar;
                            break;
                        }
                }
            }

            return !string.IsNullOrEmpty(data.PendingContent);
        }
        
        private bool FormatToken(TokenizeData<Token> data)
        {
            if (string.IsNullOrEmpty(data.PendingContent))
            {
                return true;
            }

            if (data.Grammar != null)
            {
                if (data.Grammar.CommentTerms != null && this.FormatCommentToken(data))
                {
                    return true;
                }

                if (data.Grammar.Numbers != null && this.FormatNumberToken(data))
                {
                    return true;
                }

                if (data.Grammar.StringTerms != null && this.FormatStringToken(data))
                {
                    return true;
                }

                if (data.Grammar.KeyTerms != null && this.FormatKeyToken(data))
                {
                    return true;
                }

                if (data.Grammar.Identifier != null && this.FormatIdentifierToken(data))
                {
                    return true;
                }
            }
            
            return false;
        }

        private bool CheckPunctuation(TokenizeData<Token> data)
        {
            if (data.Grammar == null)
            {
                return false;
            }

            return data.Grammar.Punctuation.Entries.Contains(data.CurrentChar);
        }
        
        private bool FormatCommentToken(TokenizeData<Token> data)
        {
            for (int i = 0; i < data.Grammar.CommentTerms.Count; i++)
            {
                TermComment term = data.Grammar.CommentTerms[i];
                if (!data.PendingContent.StartsWith(term.Start))
                {
                    continue;
                }

                var token = data.NewToken(term, data.PendingContent);
                
                // Rewind and read the comment, leave the rest of the data for the next token loop
                data.CurrentOffset -= data.PendingContent.Length + 1;
                token.Contents = this.ReadUntil(data, term.End);

                data.Finalize(token);
                return true;
            }

            return false;
        }

        private bool FormatKeyToken(TokenizeData<Token> data)
        {
            // Get the list of potential matches from the cache
            char key = data.PendingContent[0];
            if (!this.IsCaseSensitive)
            {
                key = char.ToLowerInvariant(key);
            }

            IList<TermKey> cache;
            if (!data.KeyCache.TryGetValue(key, out cache))
            {
                return false;
            }
            
            // Get rid of entries that can not match for length
            int maxLength = data.PendingContent.Length;
            IList<TermKey> potentialMatches = cache.Where(x => x.Keyword.Length <= maxLength).ToList();

            if (potentialMatches.Count <= 0)
            {
                return false;
            }
            
            if (potentialMatches.Count == 1)
            {
                var comparison = this.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                if (potentialMatches[0].Keyword.Equals(data.PendingContent.Substring(0, potentialMatches[0].Keyword.Length), comparison))
                {
                    var token = data.NewToken(potentialMatches[0], potentialMatches[0].Keyword);
                    data.Finalize(token);
                    return true;
                }

                return false;
            }

            IList<TermKey> closedMatches = new List<TermKey>();
            this.FilterKeyTokenMatches(data, potentialMatches, closedMatches);
            
            if (closedMatches.Count == 1)
            {
                var token = data.NewToken(closedMatches[0], closedMatches[0].Keyword);
                data.Finalize(token);
                return true;
            }
            
            if (potentialMatches.Count > 1)
            {
                Logger.Error("Conflicting keyword: " + data.PendingContent);
            }
            
            return false;
        }

        private void FilterKeyTokenMatches(TokenizeData<Token> data, IList<TermKey> potentialMatches, IList<TermKey> closedMatches)
        {
            // Now we check the actual matches
            for (int i = 1; i <= data.PendingContent.Length; i++)
            {
                bool nextStep = true;
                string termValue = data.PendingContent.Substring(0, i);
                if (!this.IsCaseSensitive)
                {
                    termValue = termValue.ToLowerInvariant();
                }

                IList<TermKey> checkList = new List<TermKey>(potentialMatches);
                for (int index = 0; index < checkList.Count; index++)
                {
                    string keyword = checkList[index].Keyword;
                    if (!this.IsCaseSensitive)
                    {
                        keyword = keyword.ToLowerInvariant();
                    }

                    if (!keyword.StartsWith(termValue))
                    {
                        potentialMatches.Remove(checkList[index]);
                    }
                    else if (keyword.Length <= i || keyword.Length == termValue.Length)
                    {
                        if (nextStep)
                        {
                            // This makes sure we keep the longest match
                            closedMatches.Clear();
                        }

                        closedMatches.Add(checkList[index]);
                        potentialMatches.Remove(checkList[index]);
                        nextStep = false;
                    }
                }

                if (potentialMatches.Count <= 0 || i >= data.PendingContent.Length)
                {
                    return;
                }
            }
        }

        private bool FormatIdentifierToken(TokenizeData<Token> data)
        {
            string content = string.Empty;
            for (int i = 0; i < data.PendingContent.Length; i++)
            {
                char current = data.PendingContent[i];
                if (content.Length == 0)
                {
                    if (!data.Grammar.Identifier.StartCharacters.Contains(current))
                    {
                        // We do not match identifier at all
                        return false;
                    }
                }
                else
                {
                    if (!data.Grammar.Identifier.FullCharacters.Contains(current))
                    {
                        break;
                    }
                }

                content += current;
            }

            Token token = data.IdentifierKeyCache.ContainsKey(content) 
                ? data.NewToken(data.IdentifierKeyCache[content], content) 
                : data.NewToken(data.Grammar.Identifier, content);
            
            data.Finalize(token);
            return true;
        }

        private bool FormatNumberToken(TokenizeData<Token> data)
        {
            if (data.Grammar.Numbers == null)
            {
                return false;
            }

            int tokenLength = 0;
            int numberCount = 0;
            string extension = null;
            for (int i = 0; i < data.PendingContent.Length; i++)
            {
                tokenLength++;

                if ((data.PendingContent[i] < 48) || data.PendingContent[i] > 57)
                {
                    if (data.PendingContent[i] == data.Grammar.Numbers.FloatDelimiter)
                    {
                        continue;
                    }

                    // No numbers means we don't need to check any further
                    if (numberCount <= 0)
                    {
                        return false;
                    }

                    extension = data.PendingContent.Substring(i, data.PendingContent.Length - i).ToLowerInvariant();
                    if (!data.Grammar.Numbers.Extensions.Contains(extension))
                    {
                        tokenLength--;
                        extension = null;
                    }

                    break;
                }

                numberCount++;
            }

            string numberString = data.PendingContent.Substring(0, tokenLength);
            if (numberString.EndsWith(data.Grammar.Numbers.FloatDelimiter.ToString(CultureInfo.InvariantCulture)))
            {
                return false;
            }

            var token = data.NewToken(data.Grammar.Numbers, numberString);
            token.Tag = extension;
            data.Finalize(token);

            return true;
        }

        private bool FormatStringToken(TokenizeData<Token> data)
        {
            for (int i = 0; i < data.Grammar.StringTerms.Count; i++)
            {
                TermString term = data.Grammar.StringTerms[i];
                if (data.PendingContent[0] != term.Character)
                {
                    continue;
                }

                var token = data.NewToken(term, data.PendingContent);
                data.CurrentOffset = data.CurrentOffset - data.PendingContent.Length + 1;
                token.Contents = term.Character + this.ReadUntil(data, term.Character, term.EscapeChar);
                data.PendingContent = string.Empty;
                data.Finalize(token);
                return true;
            }

            return false;
        }

        private string ReadUntil(TokenizeData<Token> data, char target, char? escape = null)
        {
            string content = string.Empty;
            int inEscape = 0;
            while (data.CurrentOffset < data.SourceData.Length)
            {
                char current = data.SourceData[data.CurrentOffset++];
                if (inEscape <= 0)
                {
                    if (escape != null && current == escape)
                    {
                        inEscape = 1;
                    }
                    else if (target == current)
                    {
                        return content + current;
                    }
                }
                else
                {
                    inEscape--;
                }

                content += current;
            }

            throw new InvalidOperationException("ReadUntil did not match the required patterns!" + target);
        }

        private string ReadUntil(TokenizeData<Token> data, string[] patterns, char[] escapes = null)
        {
            string content = string.Empty;
            int inEscape = 0;
            while (data.CurrentOffset < data.SourceData.Length)
            {
                char current = data.SourceData[data.CurrentOffset++];
                if (inEscape <= 0)
                {
                    if (escapes != null && escapes.Contains(current))
                    {
                        inEscape = 1;
                    }
                    else if (patterns.Any(t => content.EndsWith(t)))
                    {
                        return content + current;
                    }
                }
                else
                {
                    inEscape--;
                }

                content += current;
            }

            throw new InvalidOperationException("ReadUntil did not match the required patterns!" + patterns);
        }
    }
}
