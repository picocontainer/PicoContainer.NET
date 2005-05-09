using System.Collections;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.Tck;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class DefaultPicoContainerTestCase : AbstractPicoContainerTestCase
	{
		protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent)
		{
			return new DefaultPicoContainer(parent);
		}

		protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent, ILifecycleManager lifecycleManager)
		{
			return new DefaultPicoContainer(new DefaultComponentAdapterFactory(), parent, lifecycleManager);
		}

		[Test]
		public void BasicInstantiationAndContainment()
		{
			DefaultPicoContainer pico = (DefaultPicoContainer) CreatePicoContainerWithTouchableAndDependsOnTouchable();

			Assert.IsTrue(typeof (ITouchable).IsAssignableFrom(
				pico.GetComponentAdapterOfType(typeof (ITouchable)).ComponentImplementation));
		}

		[Test]
		public void ComponentInstancesFromParentsAreNotDirectlyAccessible()
		{
			IMutablePicoContainer a = new DefaultPicoContainer();
			IMutablePicoContainer b = new DefaultPicoContainer(a);
			IMutablePicoContainer c = new DefaultPicoContainer(b);

			object ao = new object();
			object bo = new object();
			object co = new object();

			a.RegisterComponentInstance("a", ao);
			b.RegisterComponentInstance("b", bo);
			c.RegisterComponentInstance("c", co);

			Assert.AreEqual(1, a.ComponentInstances.Count);
			Assert.AreEqual(1, b.ComponentInstances.Count);
			Assert.AreEqual(1, c.ComponentInstances.Count);
		}

		[Test]
		[ExpectedException(typeof (UnsatisfiableDependenciesException))]
		public void UpDownDependenciesCannotBeFollowed()
		{
			IMutablePicoContainer parent = CreatePicoContainer();
			IMutablePicoContainer child = CreatePicoContainer(parent);

			// ComponentF -> ComponentA -> ComponentB+ComponentC
			child.RegisterComponentImplementation(typeof (ComponentF));
			parent.RegisterComponentImplementation(typeof (ComponentA));
			child.RegisterComponentImplementation(typeof (ComponentB));
			child.RegisterComponentImplementation(typeof (ComponentC));

			// This should fail
			child.GetComponentInstance(typeof (ComponentF));
		}

		[Test]
		public void ComponentsCanBeRemovedByInstance()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			pico.RegisterComponentImplementation(typeof (ArrayList));
			IList list = (IList) pico.GetComponentInstanceOfType(typeof (IList));
			pico.UnregisterComponentByInstance(list);
			Assert.AreEqual(0, pico.ComponentAdapters.Count);
			Assert.AreEqual(0, pico.ComponentInstances.Count);
			Assert.IsNull(pico.GetComponentInstanceOfType(typeof (IList)));
		}

		/// <summary>
		/// When pico tries to resolve DecoratedTouchable it find as dependency itself and SimpleTouchable.
		/// Problem is basically the same as above. Pico should not consider self as solution.
		/// 
		/// JS
		/// fixed it ( PICO-222 )
		/// KP
		/// </summary>
		[Test]
		public void UnambiguouSelfDependency()
		{
			IMutablePicoContainer pico = CreatePicoContainer(null);
			pico.RegisterComponentImplementation(typeof (SimpleTouchable));
			pico.RegisterComponentImplementation(typeof (DecoratedTouchable));
			ITouchable t = (ITouchable) pico.GetComponentInstance(typeof (DecoratedTouchable));
			Assert.IsNotNull(t);
		}

		public class Thingie
		{
			public Thingie(IList c)
			{
				Assert.IsNotNull(c);
			}
		}

		[Test]
		public void ThangCanBeInstantiatedWithArrayList()
		{
			IMutablePicoContainer pico = new DefaultPicoContainer();
			pico.RegisterComponentImplementation(typeof (Thingie));
			pico.RegisterComponentImplementation(typeof (ArrayList));
			Assert.IsNotNull(pico.GetComponentInstance(typeof (Thingie)));
		}

		public class Service
		{
		}

		public class TransientComponent
		{
			public Service service;

			public TransientComponent(Service service)
			{
				this.service = service;
			}
		}

		[Test]
		public void DefaultPicoContainerReturnsNewInstanceForEachCallWhenUsingTransientComponentAdapter()
		{
			DefaultPicoContainer picoContainer = new DefaultPicoContainer();
			picoContainer.RegisterComponentImplementation(typeof (Service));
			picoContainer.RegisterComponent(new ConstructorInjectionComponentAdapter(typeof (TransientComponent), typeof (TransientComponent)));
			TransientComponent c1 = (TransientComponent) picoContainer.GetComponentInstance(typeof (TransientComponent));
			TransientComponent c2 = (TransientComponent) picoContainer.GetComponentInstance(typeof (TransientComponent));
			Assert.IsFalse(c1 == c2);
			Assert.AreSame(c1.service, c2.service);
		}

		public class DependsOnCollection
		{
			public DependsOnCollection(ICollection c)
			{
			}
		}

		[Test]
		public void ShouldProvideInfoAboutDependingWhenAmbiguityHappens()
		{
			IMutablePicoContainer pico = CreatePicoContainer(null);
			pico.RegisterComponentInstance(new ArrayList());
			pico.RegisterComponentInstance(new Stack());
			pico.RegisterComponentImplementation(typeof (DependsOnCollection));
			try
			{
				pico.GetComponentInstanceOfType(typeof (DependsOnCollection));
				Assert.Fail();
			}
			catch (AmbiguousComponentResolutionException expected)
			{
				Assert.AreEqual(typeof (DependsOnCollection).FullName
					+ " has ambiguous dependency on System.Collections.ICollection, resolves to "
					+ "multiple classes: [System.Collections.ArrayList, System.Collections.Stack]"
				                , expected.Message);
			}
		}

	}
}