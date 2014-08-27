namespace CarbonCore.GrammarParser.Contracts
{
    using System.Collections.Generic;
    using System.IO;

    public interface ITokenizer<T> where T : IToken
    {
        bool IsCaseSensitive { get; set; }

        IList<T> Tokenize(IGrammar grammar, StreamReader reader);
        IList<T> Tokenize(IGrammar grammar, string data);
    }
}
