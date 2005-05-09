using System.Collections;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Alternatives;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
	[TestFixture]
	public class ImplementationHidingCachingPicoContainerTestCase : AbstractImplementationHidingPicoContainerTestCase
	{
		protected override IMutablePicoContainer CreateImplementationHidingPicoContainer()
		{
			return new ImplementationHidingCachingPicoContainer();
		}

		protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent, ILifecycleManager lifecycleManager)
		{
			return new ImplementationHidingCachingPicoContainer(new CachingComponentAdapterFactory(new DefaultComponentAdapterFactory()), parent, lifecycleManager);
		}

		protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent)
		{
			return new ImplementationHidingCachingPicoContainer(parent);
		}

		[Test]
		public void UsageOfADifferentComponentAdapterFactory()
		{
			// Jira bug 212 - logical opposite
			IMutablePicoContainer parent = new DefaultPicoContainer();
			ImplementationHidingCachingPicoContainer pico = new ImplementationHidingCachingPicoContainer(new ConstructorInjectionComponentAdapterFactory(), parent);
			pico.RegisterComponentImplementation(typeof (IList), typeof (ArrayList));
			IList list1 = (IList) pico.GetComponentInstanceOfType(typeof (IList));
			IList list2 = (IList) pico.GetComponentInstanceOfType(typeof (IList));
			Assert.IsNotNull(list1);
			Assert.IsNotNull(list2);
			Assert.IsTrue(list1 == list2);
		}
	}
}