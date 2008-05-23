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

using System;

namespace PicoContainer.Defaults
{
    /// <summary>
    /// Base for implementing ComponentAdapters
    /// </summary>
    [Serializable]
    public abstract class AbstractComponentAdapter : IComponentAdapter
    {
        private readonly Type componentImplementation;
        private readonly object componentKey;
        private IPicoContainer container;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentKey">The component's key</param>
        /// <param name="componentImplementation">The component implementing type</param>
        protected AbstractComponentAdapter(object componentKey, Type componentImplementation)
        {
            if (componentImplementation == null)
            {
                throw new NullReferenceException("componentImplementation");
            }
            this.componentKey = componentKey;
            this.componentImplementation = componentImplementation;
            CheckTypeCompatibility();
            CheckConcrete();
        }

        #region IComponentAdapter Members

        /// <summary>
        /// The key of the component
        /// </summary>
        public object ComponentKey
        {
            get
            {
                if (componentKey == null)
                {
                    throw new NullReferenceException("componentKey");
                }
                return componentKey;
            }
        }

        /// <summary>
        /// The component implementing type
        /// </summary>
        public Type ComponentImplementation
        {
            get { return componentImplementation; }
        }

        /// <summary>
        ///  Property containing the container in which this instance is registered, called by the container upon registration
        /// </summary>
        public IPicoContainer Container
        {
            get { return container; }

            set { container = value; }
        }

        public abstract object GetComponentInstance(IPicoContainer container);

        /// <summary>
        /// Verify that all dependencies for this adapter can be satisifed.
        /// </summary>
        /// <exception cref="PicoContainer.PicoIntrospectionException">if the verification failed</exception>
        public abstract void Verify(IPicoContainer container);

        #endregion

        private void CheckTypeCompatibility()
        {
            Type componentType = componentKey as Type;
            if (componentType != null)
            {
                if (!componentType.IsAssignableFrom(componentImplementation))
                {
                    throw new AssignabilityRegistrationException(componentType, componentImplementation);
                }
            }
        }

        protected void CheckConcrete()
        {
            if (componentImplementation.IsInterface || componentImplementation.IsAbstract)
            {
                throw new NotConcreteRegistrationException(componentImplementation);
            }
        }

        /// <summary>
        /// The string representation of the ComponentAdapter
        /// </summary>
        /// <returns>The name</returns>
        public override string ToString()
        {
            return GetType().Name + "[" + ComponentKey + "]";
        }
    }
}