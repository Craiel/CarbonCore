namespace CarbonCore.GrammarParser.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.Terms;

    public interface IGrammar
    {
        TermIdentifier Identifier { get; }
        TermPunctuation Punctuation { get; }
        TermNumber Numbers { get; }

        IList<TermKey> KeyTerms { get; }
        IList<TermIdentifierKey> IdentifierKeyTerms { get; }
        IList<TermComment> CommentTerms { get; }
        IList<TermString> StringTerms { get; }
    }
}
