// ReSharper disable UnusedMember.Global
namespace Gu.Localization.TestStub
{
    public class Foo
    {
        public Foo()
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var text = "One resource";
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
    }
}
