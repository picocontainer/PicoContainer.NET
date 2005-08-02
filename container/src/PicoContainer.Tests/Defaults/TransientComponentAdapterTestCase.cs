using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;

namespace PicoContainer.Defaults
{
	/// <summary>
	/// Summary description for TransientIComponentAdapterTestCase.
	/// </summary>
	[TestFixture]
	public class TransientIComponentAdapterTestCase
	{
		[Test]
		public void NonCachingComponentAdapterReturnsNewInstanceOnEachCallToGetComponentInstance()
		{
			ConstructorInjectionComponentAdapter componentAdapter = new ConstructorInjectionComponentAdapter("blah", typeof (object));
			object o1 = componentAdapter.GetComponentInstance(null);
			object o2 = componentAdapter.GetComponentInstance(null);
			Assert.IsNotNull(o1);
			Assert.IsFalse(o1.Equals(o2));
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
		public void DefaultPicoContainerReturnsNewInstanceForEachCallWhenUsingTransientIComponentAdapter()
		{
			DefaultPicoContainer picoContainer = new DefaultPicoContainer();
			picoContainer.RegisterComponentImplementation(typeof (Service));
			picoContainer.RegisterComponent(new ConstructorInjectionComponentAdapter(typeof (TransientComponent)));
			TransientComponent c1 = (TransientComponent) picoContainer.GetComponentInstance(typeof (TransientComponent));
			TransientComponent c2 = (TransientComponent) picoContainer.GetComponentInstance(typeof (TransientComponent));
			Assert.IsFalse(c1.Equals(c2));
			Assert.AreSame(c1.service, c2.service);
		}

		public class A
		{
			public A()
			{
				Assert.Fail("verification should not instantiate");
			}
		}

		public class B
		{
			public B(A a)
			{
				Assert.Fail("verification should not instantiate");
			}
		}


		[Test]
		public void SuccessfulVerificationWithNoDependencies()
		{
			InstantiatingComponentAdapter componentAdapter = new ConstructorInjectionComponentAdapter("foo", typeof (A));
			componentAdapter.Verify(componentAdapter.Container);
		}

		[Test]
		public void FailingVerificationWithUnsatisfiedDependencies()
		{
			IComponentAdapter componentAdapter = new ConstructorInjectionComponentAdapter("foo", typeof (B));
			componentAdapter.Container = new DefaultPicoContainer();
			try
			{
				componentAdapter.Verify(componentAdapter.Container);
				Assert.Fail();
			}
			catch (UnsatisfiableDependenciesException)
			{
			}
		}

	}
}