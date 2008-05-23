using System;
using NUnit.Framework;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    [TestFixture]
    public class ImplementationHidingComponentAdapterTestCase
    {
        [Test]
        public void MultipleInterfacesCanBeHidden()
        {
            Type[] interfaces = new Type[] {typeof (InterfaceOne), typeof (InterfaceTwo)};
            Type implementation = typeof (FooBar);

            IComponentAdapter componentAdapter = new ConstructorInjectionComponentAdapter(
                interfaces,
                implementation);

            ImplementationHidingComponentAdapter ihca = new ImplementationHidingComponentAdapter(componentAdapter, true);
            Object comp = ihca.GetComponentInstance(null);
            Assert.IsNotNull(comp);
            Assert.IsTrue(comp is InterfaceOne);
            Assert.IsTrue(comp is InterfaceTwo);

            // Lets makes sure the actual object is invoked through the proxy
            Assert.IsTrue(((InterfaceOne) comp).MethodOne());
            Assert.IsFalse(((InterfaceTwo) comp).MethodTwo());
        }

        [Test]
        [ExpectedException(typeof (PicoIntrospectionException))]
        public void NonInterfaceInArrayCantBeHidden()
        {
            IComponentAdapter ca =
                new ConstructorInjectionComponentAdapter(new Type[] {typeof (string)}, typeof (FooBar));
            ImplementationHidingComponentAdapter ihca = new ImplementationHidingComponentAdapter(ca, true);
            ihca.GetComponentInstance(null);
            Assert.Fail("Oh no.");
        }

        [Test]
        [ExpectedException(typeof (PicoIntrospectionException))]
        public void ShouldThrowExceptionWhenAccessingNonInterfaceKeyedComponentInStrictMode()
        {
            IComponentAdapter ca = new ConstructorInjectionComponentAdapter("ww", typeof (FooBar));
            ImplementationHidingComponentAdapter ihca = new ImplementationHidingComponentAdapter(ca, true);
            ihca.GetComponentInstance(null);
            Assert.Fail("Oh no.");
        }
    }

    public interface InterfaceOne
    {
        bool MethodOne();
    }

    public interface InterfaceTwo
    {
        bool MethodTwo();
    }

    public class FooBar : InterfaceOne, InterfaceTwo
    {
        #region InterfaceOne Members

        public bool MethodOne()
        {
            return true;
        }

        #endregion

        #region InterfaceTwo Members

        public bool MethodTwo()
        {
            return false;
        }

        #endregion
    }
}