using System.Collections;
using NUnit.Framework;
using PicoContainer.Defaults;
using PicoContainer.Tck;

namespace PicoContainer.Alternatives
{
    /// <summary>
    /// Summary description for CachingPicoContainerTestCase.
    /// </summary>
    [TestFixture]
    public class CachingPicoContainerTestCase : AbstractPicoContainerTestCase
    {
        protected IMutablePicoContainer CreateImplementationHidingPicoContainer()
        {
            return new CachingPicoContainer();
        }

        protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent)
        {
            return new CachingPicoContainer(parent);
        }

        protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent,
                                                                     ILifecycleManager lifecycleManager)
        {
            return new CachingPicoContainer(parent, lifecycleManager);
        }

        [Test]
        public void UsageOfADifferentComponentAdapterFactory()
        {
            // Jira bug 212 - logical opposite
            IMutablePicoContainer parent = new DefaultPicoContainer();
            CachingPicoContainer pico =
                new CachingPicoContainer(new ConstructorInjectionComponentAdapterFactory(), parent);
            pico.RegisterComponentImplementation(typeof (IList), typeof (ArrayList));
            IList list1 = (IList) pico.GetComponentInstanceOfType(typeof (IList));
            IList list2 = (IList) pico.GetComponentInstanceOfType(typeof (IList));
            Assert.IsNotNull(list1);
            Assert.IsNotNull(list2);
            Assert.IsTrue(list1 == list2);
        }
    }
}