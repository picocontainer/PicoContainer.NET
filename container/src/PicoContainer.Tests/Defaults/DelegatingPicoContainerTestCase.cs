using NUnit.Framework;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
    [TestFixture]
    public class DelegatingPicoContainerTestCase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            parent = new DefaultPicoContainer();
            child = new DefaultPicoContainer(parent);
        }

        #endregion

        private IMutablePicoContainer parent;
        private DefaultPicoContainer child;

        [Test]
        public void TestChildGetsFromParent()
        {
            parent.RegisterComponentImplementation(typeof (SimpleTouchable));
            child.RegisterComponentImplementation(typeof (DependsOnTouchable));
            DependsOnTouchable dependsOnTouchable =
                (DependsOnTouchable) child.GetComponentInstance(typeof (DependsOnTouchable));

            Assert.IsNotNull(dependsOnTouchable);
        }

        [Test]
        public void TestChildOverridesParent()
        {
            parent.RegisterComponentImplementation(typeof (SimpleTouchable));
            child.RegisterComponentImplementation(typeof (SimpleTouchable));

            SimpleTouchable parentTouchable = (SimpleTouchable) parent.GetComponentInstance(typeof (SimpleTouchable));
            SimpleTouchable childTouchable = (SimpleTouchable) child.GetComponentInstance(typeof (SimpleTouchable));
            Assert.AreEqual(1, child.ComponentInstances.Count);
            Assert.IsFalse(parentTouchable.Equals(childTouchable));
        }

        [Test]
        [ExpectedException(typeof (UnsatisfiableDependenciesException))]
        public void TestParentDoesntGetFromChild()
        {
            child.RegisterComponentImplementation(typeof (SimpleTouchable));
            parent.RegisterComponentImplementation(typeof (DependsOnTouchable));

            parent.GetComponentInstance(typeof (DependsOnTouchable));
        }
    }
}