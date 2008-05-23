using NUnit.Framework;
using PicoContainer.Tck;

namespace PicoContainer.Defaults
{
    [TestFixture]
    public class DefaultLazyInstantiationTestCase : AbstractLazyInstantiationTestCase
    {
        protected override IMutablePicoContainer createPicoContainer()
        {
            return new DefaultPicoContainer();
        }
    }
}