using NUnit.Framework;
using PicoContainer.Defaults;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class DefaultPicoContainerUnregisterComponentTestCase
	{
		private DefaultPicoContainer picoContainer;

		[SetUp]
		protected void SetUp()
		{
			picoContainer = new DefaultPicoContainer();
		}

		[Test]
		public void TestCannotInstantiateAnUnregisteredComponent()
		{
			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));

			Assert.IsTrue(picoContainer.ComponentInstances.Count == 0);
		}

		[Test]
		public void TestCanInstantiateReplacedComponent()
		{
			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));

			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (AlternativeTouchable));

			Assert.AreEqual(1, picoContainer.ComponentInstances.Count);
		}

		[Test]
		public void TestUnregisterAfterInstantiateComponents()
		{
			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));
			Assert.IsNull(picoContainer.GetComponentInstance(typeof (ITouchable)));
		}

		[Test]
		public void TestReplacedInstantiatedComponentHasCorrectClass()
		{
			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
			object o = picoContainer.ComponentInstances;
			picoContainer.UnregisterComponent(typeof (ITouchable));

			picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (AlternativeTouchable));
			object component = picoContainer.ComponentInstances[0];

			Assert.AreEqual(typeof (AlternativeTouchable), component.GetType());
		}

	}
}