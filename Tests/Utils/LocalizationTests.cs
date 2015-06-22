namespace CarbonCore.Tests.Utils
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.I18N;

    using NUnit.Framework;

    [TestFixture]
    public class LocalizationTests
    {
        private static readonly IList<string> TestStrings = new List<string>
                                                            {
                                                                "This is a test string",
                                                                "This is another string",
                                                                "1234"
                                                            };

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GeneralTest()
        {
            Assert.NotNull(Localization.CurrentCulture);
            
            // run through first to test
            foreach (string testString in TestStrings)
            {
                testString.Localized();
            }

            Localization.CurrentCulture = LocaleConstants.LocaleGerman;
            foreach (string testString in TestStrings)
            {
                string localizedTest = testString.Localized();
                Localization.SetString(localizedTest, localizedTest.ToReverse());
            }

            Localization.SaveDictionaries();
            Localization.ReloadDictionaries();

            foreach (string testString in TestStrings)
            {
                Assert.AreNotEqual(testString.Localized(), testString, "Changed dictionary value must be loaded properly");
            }
        }
    }
}
