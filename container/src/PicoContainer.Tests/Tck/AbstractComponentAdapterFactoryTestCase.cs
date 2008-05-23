/*****************************************************************************
 * Copyright (C) PicoContainer Organization. All rights reserved.            *
 * ------------------------------------------------------------------------- *
 * The software in this package is published under the terms of the BSD      *
 * style license a copy of which has been included with this distribution in *
 * the license.txt file.                                                     *
 *                                                                           *
 * Idea by Rachel Davies, Original code by Aslak Hellesoy and Paul Hammant   *
 * C# port by Maarten Grootendorst                                           *
 *****************************************************************************/
using NUnit.Framework;
using PicoContainer.Defaults;
using PicoContainer.TestModel;

namespace PicoContainer.Tck
{
    [TestFixture]
    public abstract class AbstractComponentAdapterFactoryTestCase
    {
        #region Setup/Teardown

        [SetUp]
        public virtual void SetUp()
        {
            picoContainer = new DefaultPicoContainer();
        }

        #endregion

        protected DefaultPicoContainer picoContainer;

        protected abstract IComponentAdapterFactory CreateComponentAdapterFactory();

        [Test]
        public void RegisterComponent()
        {
            IComponentAdapter componentAdapter =
                CreateComponentAdapterFactory().CreateComponentAdapter(typeof (ITouchable), typeof (SimpleTouchable),
                                                                       null);

            picoContainer.RegisterComponent(componentAdapter);

            Assert.IsTrue(picoContainer.ComponentAdapters.Contains(componentAdapter));
        }

        [Test]
        public void TestEquals()
        {
            IComponentAdapter componentAdapter =
                CreateComponentAdapterFactory().CreateComponentAdapter(typeof (ITouchable), typeof (SimpleTouchable),
                                                                       null);
            Assert.AreEqual(componentAdapter, componentAdapter);
        }

        [Test]
        public void UnRegisterComponent()
        {
            IComponentAdapter componentAdapter =
                CreateComponentAdapterFactory().CreateComponentAdapter(typeof (ITouchable), typeof (SimpleTouchable),
                                                                       null);

            picoContainer.RegisterComponent(componentAdapter);
            Assert.IsNotNull(picoContainer.UnregisterComponent(typeof (ITouchable)));
            Assert.IsFalse(picoContainer.ComponentAdapters.Contains(componentAdapter));
        }
    }
}