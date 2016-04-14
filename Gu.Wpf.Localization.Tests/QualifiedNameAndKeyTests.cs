namespace Gu.Wpf.Localization.Tests
{
    using NUnit.Framework;

    public class QualifiedNameAndKeyTests
    {
        [TestCase("p:Resources.Key")]
        public void Parse(string s)
        {
            var actual = QualifiedNameAndKey.Parse(s);
            Assert.AreEqual("p:Resources", actual.QualifiedName);
            Assert.AreEqual("Key", actual.Key);
        }

        [Test]
        public void Caches()
        {
            var actual1 = QualifiedNameAndKey.Parse("p:Resources.Key");
            var actual2 = QualifiedNameAndKey.Parse("p:Resources.Key");
            Assert.AreSame(actual1, actual2);
        }
    }
}
