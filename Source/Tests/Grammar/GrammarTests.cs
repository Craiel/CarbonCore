﻿namespace CarbonCore.Tests.Edge.Grammar
{
    using System.Collections.Generic;
    using System.Linq;
    
    using CarbonCore.GrammarParser;
    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.IoC;
    using CarbonCore.GrammarParser.Tokenize;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Edge.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class GrammarTests
    {
        private ICarbonContainer container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerAutofacBuilder.Build<GrammarParserModule>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void CommandLineGrammarTests()
        {
            const string TestLine = "-first -second -parameterFirst=Test -parameterSecond=Test2 --doubleDash --doubleDashValue:testme -stringparam=\"This is a long string\" | PipedIntoIgnore";

            var grammar = this.container.Resolve<ICommandLineGrammar>();
            var tokenizer = new Tokenizer();
            IList<Token> tokens = tokenizer.Tokenize(grammar, TestLine);

            Assert.AreEqual(22, tokens.Count, "Tokens expected to be 18");

            int identifiers = tokens.Count(x => x.Term.Type == TermType.Identifier);
            int keys = tokens.Count(x => x.Term.Type == TermType.Key);

            Assert.AreEqual(10, identifiers, "Expected 10 Identifiers");
            Assert.AreEqual(11, keys, "Expected 11 Keys");
        }

        [Test]
        public void SqlGrammarTests()
        {
            // From http://www.sqlexamples.info/SQL/subquerybad.htm
            const string TestStatement = @"SELECT a.CustomerID, c.CustomerName, c.phone1 
FROM ((Invoice AS a INNER JOIN InvLines AS b ON a.DocKey=b.DocKey) 
INNER JOIN Customers AS c ON a.CustomerID = c.CustomerID) 
INNER JOIN 
(
SELECT a.ItemCode 
FROM (InvLines AS a INNER JOIN Invoice AS b ON a.DocKey=b.DocKey) 
INNER JOIN Customers AS c ON b.CustomerID = c.CustomerID 
WHERE c.CustomerName = 'John Depp' 
GROUP BY a.ItemCode 
) AS d 
ON b.ItemCode = d.ItemCode 
WHERE c.CustomerName <> 'John Depp' 
GROUP BY a.CustomerID, c.CustomerName, c.phone1";

            var grammer = this.container.Resolve<ISqlGrammar>();
            var tokenizer = new Tokenizer { IsCaseSensitive = false };
            IList<Token> tokens = tokenizer.Tokenize(grammer, TestStatement);

            Assert.AreEqual(87, tokens.Count, "Expected 87 Tokens");

            int identifiers = tokens.Count(x => x.Term.Type == TermType.Identifier);
            Assert.AreEqual(33, identifiers, "Expected 33 Identifiers");
        }
    }
}
