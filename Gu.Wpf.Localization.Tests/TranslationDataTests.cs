namespace Gu.Wpf.Localization.Tests
{
    using NUnit.Framework;

    public class TranslationDataTests
    {
        [Test]
        public void DoesNotThrowWithNullManager()
        {
            const string Key = "Test";
            var translationData = new TranslationData(Key, null);
            var value = translationData.Value;
            var expected = string.Format(Gu.Wpf.Localization.Properties.Resources.NullManagerFormat, Key);
            Assert.AreEqual(expected, value);
        }
    }
}