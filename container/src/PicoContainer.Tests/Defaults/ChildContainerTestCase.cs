using PicoContainer;
using PicoContainer.Defaults;
using NUnit.Framework;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class ChildContainerTestCase
	{
		[Test]
		public void ParentContainerWithComponentWithEqualKeyShouldBeShadowedByChild()
		{
			DefaultPicoContainer parent = new DefaultPicoContainer();
			DefaultPicoContainer child = new DefaultPicoContainer(parent);

			parent.RegisterComponentImplementation("key", typeof (AlternativeTouchable));
			child.RegisterComponentImplementation("key", typeof (SimpleTouchable));
			child.RegisterComponentImplementation(typeof (DependsOnTouchable));

			DependsOnTouchable dot = (DependsOnTouchable) child.GetComponentInstanceOfType(typeof (DependsOnTouchable));
			Assert.AreEqual(typeof (SimpleTouchable), dot.getTouchable().GetType());
		}

		[Test]
		public void ParentComponentRegisteredAsClassShouldBePreffered()
		{
			DefaultPicoContainer parent = new DefaultPicoContainer();
			DefaultPicoContainer child = new DefaultPicoContainer(parent);

			parent.RegisterComponentImplementation(typeof (ITouchable), typeof (AlternativeTouchable));
			child.RegisterComponentImplementation("key", typeof (SimpleTouchable));
			child.RegisterComponentImplementation(typeof (DependsOnTouchable));

			DependsOnTouchable dot = (DependsOnTouchable) child.GetComponentInstanceOfType(typeof (DependsOnTouchable));
			Assert.AreEqual(typeof (AlternativeTouchable), dot.getTouchable().GetType());
		}

		[Test]
		public void ResolveFromParentByType()
		{
			IMutablePicoContainer parent = new DefaultPicoContainer();
			parent.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));

			IMutablePicoContainer child = new DefaultPicoContainer(parent);
			child.RegisterComponentImplementation(typeof (DependsOnTouchable));

			Assert.IsNotNull(child.GetComponentInstance(typeof (DependsOnTouchable)));
		}

		[Test]
		public void ResolveFromParentByKey()
		{
			IMutablePicoContainer parent = new DefaultPicoContainer();
			parent.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));

			IMutablePicoContainer child = new DefaultPicoContainer(parent);
			child.RegisterComponentImplementation(typeof (DependsOnTouchable), typeof (DependsOnTouchable),
			                                      new IParameter[] {new ComponentParameter(typeof (ITouchable))});

			Assert.IsNotNull(child.GetComponentInstance(typeof (DependsOnTouchable)));
		}

		[Test]
		public void ResolveFromGrandParentByType()
		{
			IMutablePicoContainer grandParent = new DefaultPicoContainer();
			grandParent.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));

			IMutablePicoContainer parent = new DefaultPicoContainer(grandParent);

			IMutablePicoContainer child = new DefaultPicoContainer(parent);
			child.RegisterComponentImplementation(typeof (DependsOnTouchable));

			Assert.IsNotNull(child.GetComponentInstance(typeof (DependsOnTouchable)));
		}

		[Test]
		public void ResolveFromGrandParentByKey()
		{
			IMutablePicoContainer grandParent = new DefaultPicoContainer();
			grandParent.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));

			IMutablePicoContainer parent = new DefaultPicoContainer(grandParent);

			IMutablePicoContainer child = new DefaultPicoContainer(parent);
			child.RegisterComponentImplementation(typeof (DependsOnTouchable), typeof (DependsOnTouchable),
			                                      new IParameter[] {new ComponentParameter(typeof (ITouchable))});

			Assert.IsNotNull(child.GetComponentInstance(typeof (DependsOnTouchable)));
		}

	}
}