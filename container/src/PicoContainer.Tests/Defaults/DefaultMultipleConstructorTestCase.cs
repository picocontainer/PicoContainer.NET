using NUnit.Framework;
using PicoContainer.Tck;

namespace PicoContainer.Defaults
{
    [TestFixture]
    public class DefaultMultipleConstructorTestCase : AbstractMultipleConstructorTestCase
    {
        protected override IMutablePicoContainer createPicoContainer()
        {
            return new DefaultPicoContainer();
        }
    }
}