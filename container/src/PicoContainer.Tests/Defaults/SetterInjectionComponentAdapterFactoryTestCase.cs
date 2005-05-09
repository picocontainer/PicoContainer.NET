using System;
using PicoContainer.Tck;
using PicoContainer.Defaults;
using NUnit.Framework;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class SetterInjectionComponentAdapterFactoryTestCase : AbstractComponentAdapterFactoryTestCase
	{
		[SetUp]
		public override void SetUp()
		{
			picoContainer = new DefaultPicoContainer(CreateComponentAdapterFactory());
		}

		protected override IComponentAdapterFactory CreateComponentAdapterFactory()
		{
			return new SetterInjectionComponentAdapterFactory();
		}

		public interface Bean
		{
		}

		public class NamedBean : Bean
		{
			private String name;

			public String Name
			{
				get { return name; }
				set { name = value; }
			}
		}

		public class NamedBeanWithPossibleDefault : NamedBean
		{
			private bool byDefault = false;

			public NamedBeanWithPossibleDefault()
			{
			}

			public NamedBeanWithPossibleDefault(String name)
			{
				Name = name;
				byDefault = true;
			}

			public bool ByDefault
			{
				get { return byDefault; }
			}
		}

		public class NoBean : NamedBean
		{
			public NoBean(String name)
			{
				Name = name;
			}
		}

		[Test]
		public void ContainerUsesStandardConstructor() 
		{
			picoContainer.RegisterComponentImplementation(typeof(Bean), typeof(NamedBeanWithPossibleDefault));
			picoContainer.RegisterComponentInstance("Tom");
			NamedBeanWithPossibleDefault bean = (NamedBeanWithPossibleDefault) picoContainer.GetComponentInstance(typeof(Bean));
			Assert.IsFalse(bean.ByDefault);
		}

		[Test]
		[ExpectedException(typeof(PicoInvocationTargetInitializationException))]
		public void ContainerUsesOnlyStandardConstructor() 
		{
			picoContainer.RegisterComponentImplementation(typeof(Bean), typeof(NoBean));
			picoContainer.RegisterComponentInstance("Tom");
			picoContainer.GetComponentInstance(typeof(Bean));
		}
	}
}