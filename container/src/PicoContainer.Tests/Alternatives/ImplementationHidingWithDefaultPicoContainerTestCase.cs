using NUnit.Framework;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    [TestFixture]
    public class ImplementationHidingWithDefaultPicoContainerTestCase :
        AbstractImplementationHidingPicoContainerTestCase
    {
        protected override IMutablePicoContainer CreateImplementationHidingPicoContainer()
        {
            return CreatePicoContainer(null);
        }

        protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent)
        {
            return
                new DefaultPicoContainer(
                    new CachingComponentAdapterFactory(
                        new ImplementationHidingComponentAdapterFactory(
                            new ConstructorInjectionComponentAdapterFactory(), false)), parent);
        }

        protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent,
                                                                     ILifecycleManager lifecycleManager)
        {
            return
                new DefaultPicoContainer(
                    new CachingComponentAdapterFactory(
                        new ImplementationHidingComponentAdapterFactory(
                            new ConstructorInjectionComponentAdapterFactory(), false)), parent, lifecycleManager);
        }
    }
}