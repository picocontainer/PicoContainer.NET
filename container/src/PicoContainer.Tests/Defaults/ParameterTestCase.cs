using System;
using NUnit.Framework;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
    [TestFixture]
    public class ParameterTestCase
    {
        [Test]
        public void ComponentParameterExcludesSelf()
        {
            DefaultPicoContainer pico = new DefaultPicoContainer();
            IComponentAdapter adapter =
                pico.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));

            Assert.IsNotNull(pico.GetComponentInstance(typeof (ITouchable)));
            ITouchable touchable =
                (ITouchable) ComponentParameter.DEFAULT.ResolveInstance(pico, adapter, typeof (ITouchable));
            Assert.IsNull(touchable);
        }

        [Test]
        public void ComponentParameterFetches()
        {
            DefaultPicoContainer pico = new DefaultPicoContainer();
            pico.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
            ComponentParameter parameter = new ComponentParameter(typeof (ITouchable));

            Assert.IsNotNull(pico.GetComponentInstance(typeof (ITouchable)));
            ITouchable touchable = (ITouchable) parameter.ResolveInstance(pico, null, typeof (ITouchable));
            Assert.IsNotNull(touchable);
        }

        [Test]
        public void ComponentParameterRespectsExpectedType()
        {
            IMutablePicoContainer picoContainer = new DefaultPicoContainer();
            IComponentAdapter adapter =
                picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
            Assert.IsNull(
                ComponentParameter.DEFAULT.ResolveInstance(picoContainer, adapter, typeof (TestFixtureAttribute)));
        }

        [Test]
        public void ConstantParameter()
        {
            Object value = new Object();
            ConstantParameter parameter = new ConstantParameter(value);
            IMutablePicoContainer picoContainer = new DefaultPicoContainer();
            Assert.AreSame(value, parameter.ResolveInstance(picoContainer, null, typeof (object)));
        }

        [Test]
        public void ConstantParameterRespectsExpectedType()
        {
            IMutablePicoContainer picoContainer = new DefaultPicoContainer();
            IParameter parameter = new ConstantParameter(new SimpleTouchable());
            IComponentAdapter adapter =
                picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
            Assert.IsFalse(parameter.IsResolvable(picoContainer, adapter, typeof (TestFixtureAttribute)));
        }

        [Test]
        public void ParameterRespectsExpectedType()
        {
            IParameter parameter = new ConstantParameter(typeof (ITouchable));
            IMutablePicoContainer picoContainer = new DefaultPicoContainer();
            Assert.IsFalse(parameter.IsResolvable(picoContainer, null, typeof (TestFixtureAttribute)));

            IComponentAdapter adapter =
                picoContainer.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));

            Assert.IsNull(
                ComponentParameter.DEFAULT.ResolveInstance(picoContainer, adapter, typeof (TestFixtureAttribute)));
        }

        [Test]
        public void TestDependsOnTouchableWithTouchableSpecifiedAsConstant()
        {
            DefaultPicoContainer pico = new DefaultPicoContainer();
            SimpleTouchable touchable = new SimpleTouchable();
            pico.RegisterComponentImplementation(typeof (DependsOnTouchable), typeof (DependsOnTouchable),
                                                 new IParameter[]
                                                     {
                                                         new ConstantParameter(touchable)
                                                     });
            object o = pico.ComponentInstances;
            Assert.IsTrue(touchable.WasTouched);
        }
    }
}