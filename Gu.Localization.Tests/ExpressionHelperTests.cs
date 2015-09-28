namespace Gu.Localization.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;

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
            var manager = ExpressionHelper.GetResourceManager(() => Properties.Resources.AllLanguages);
            Assert.AreSame(Properties.Resources.ResourceManager, manager);
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

            actual = ExpressionHelper.IsResourceKey(() => Properties.Resources.AllLanguages.Length.ToString());
            Assert.AreEqual(false, actual);

            actual = ExpressionHelper.IsResourceKey(() => Properties.Resources.Culture.EnglishName);
            Assert.AreEqual(false, actual);
        }
    }
}
