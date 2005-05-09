using System;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class DefaultComponentAdapterFactoryTestCase
	{
		protected IComponentAdapterFactory CreateComponentAdapterFactory()
		{
			return new DefaultComponentAdapterFactory();
		}

		[Test]
		public void InstantiateComponentWithNoDependencies()
		{
			IComponentAdapter componentAdapter = CreateComponentAdapterFactory()
				.CreateComponentAdapter(typeof (ITouchable), typeof (SimpleTouchable), null);

			Object comp = componentAdapter.GetComponentInstance(null);
			Assert.IsNotNull(comp);
			Assert.IsTrue(comp is SimpleTouchable);
		}

		[Test]
		public void SingleUseComponentCanBeInstantiatedByDefaultIComponentAdapter()
		{
			IComponentAdapter componentAdapter = CreateComponentAdapterFactory()
				.CreateComponentAdapter("o", typeof (object), null);
			Object component = componentAdapter.GetComponentInstance(null);
			Assert.IsNotNull(component);
		}
	}
}