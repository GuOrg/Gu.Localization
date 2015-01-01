namespace Gu.Localization.Tests
{
    using NUnit.Framework;

    public class ExpressionHelperTests
    {
        [Test]
        public void GetResourceKey()
        {
            var resourceKey = ExpressionHelper.GetResourceKey(() => Properties.Resources.AllLanguages);
            Assert.AreEqual("AllLanguages", resourceKey);
        }

        [Test]
        public void GetResourceManager()
        {
            var resourceKey = ExpressionHelper.GetResourceManager(() => Properties.Resources.AllLanguages);
            Assert.AreEqual(Properties.Resources.ResourceManager, resourceKey);
        }

        [Test]
        public void IsResourceKeyIsTrueForValidKey()
        {
            var actual = ExpressionHelper.IsResourceKey(() => Properties.Resources.AllLanguages);
            Assert.AreEqual(true, actual);
        }

        [Test]
        public void IsResourceKeyIsFalseForInvalidKey()
        {
            var actual = ExpressionHelper.IsResourceKey(() => "");
            Assert.AreEqual(false, actual);
        }
    }
}
