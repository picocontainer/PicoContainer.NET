using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class DefaultComponentRegistryTestCase
	{
		private DefaultPicoContainer picoContainer;

		[SetUp]
		protected void SetUp()
		{
			picoContainer = new DefaultPicoContainer();
		}

		[Test]
		public void RegisterComponent()
		{
			IComponentAdapter componentSpecification = CreateComponentAdapter();
			picoContainer.RegisterComponent(componentSpecification);
			Assert.IsTrue(picoContainer.ComponentAdapters.Contains(componentSpecification));
		}

		[Test]
		public void UnregisterComponent()
		{
			IComponentAdapter componentSpecification = CreateComponentAdapter();
			picoContainer.RegisterComponent(componentSpecification);
			picoContainer.UnregisterComponent(typeof (ITouchable));
			Assert.IsFalse(picoContainer.ComponentAdapters.Contains(componentSpecification));
		}

		[Test]
		public void CannotInstantiateAnUnregisteredComponent()
		{
			IComponentAdapter componentAdapter = CreateComponentAdapter();
			picoContainer.RegisterComponent(componentAdapter);
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));

			Assert.IsTrue(picoContainer.ComponentInstances.Count == 0);
		}

		[Test]
		public void CanInstantiateReplacedComponent()
		{
			IComponentAdapter componentAdapter = CreateComponentAdapter();
			picoContainer.RegisterComponent(componentAdapter);
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));
			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (AlternativeTouchable));

			Assert.AreEqual(1, picoContainer.ComponentInstances.Count, "Container should container 1 component");
		}

		[Test]
		public void UnregisterAfterInstantiateComponents()
		{
			IComponentAdapter componentAdapter = CreateComponentAdapter();
			picoContainer.RegisterComponent(componentAdapter);
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));

			Assert.IsNull(picoContainer.GetComponentInstance(typeof (ITouchable)));
		}

		[Test]
		public void ReplacedInstantiatedComponentHasCorrectClass()
		{
			IComponentAdapter componentAdapter = CreateComponentAdapter();
			picoContainer.RegisterComponent(componentAdapter);
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));

			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (AlternativeTouchable));
			object component = picoContainer.GetComponentInstance(typeof(ITouchable));

			Assert.AreEqual(typeof (AlternativeTouchable), component.GetType());
		}

		private IComponentAdapter CreateComponentAdapter()
		{
			return new ConstructorInjectionComponentAdapter(typeof (ITouchable), typeof (SimpleTouchable));
		}

	}
}