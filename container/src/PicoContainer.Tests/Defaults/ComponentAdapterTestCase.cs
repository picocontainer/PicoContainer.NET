using System;
using NUnit.Framework;

namespace PicoContainer.Defaults
{
    [TestFixture]
    public class ComponentAdapterTestCase
    {
        [Test]
        [ExpectedException(typeof (NullReferenceException))]
        public void ComponentImplementationMayNotBeNull()
        {
            new TestComponentAdapter("Key", null);
        }

        [Test]
        [ExpectedException(typeof (NullReferenceException))]
        public void ComponentKeyCanBeNullButNotRequested()
        {
            IComponentAdapter componentAdapter = new TestComponentAdapter(null, typeof (string));
            object key = componentAdapter.ComponentKey;
        }

        [Test]
        public void StringRepresentation()
        {
            IComponentAdapter componentAdapter = new TestComponentAdapter("Key", typeof (int));
            Assert.AreEqual(typeof (TestComponentAdapter).Name + "[Key]", componentAdapter.ToString());
        }
    }

    public class TestComponentAdapter : AbstractComponentAdapter
    {
        public TestComponentAdapter(Object componentKey, Type componentImplementation)
            : base(componentKey, componentImplementation)
        {
        }

        public override object GetComponentInstance(IPicoContainer container)
        {
            return null;
        }

        public override void Verify(IPicoContainer container)
        {
        }
    }
}