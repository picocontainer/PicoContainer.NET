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
    /// ComponentAdapter initializing the component only once. Reusing the component.
    /// <remarks>Components registered using this adapter can be seen as Singleton. No synchronization of calls is done.
    /// </remarks>
    /// </summary>
    [Serializable]
    public class CachingComponentAdapter : DecoratingComponentAdapter
    {
        private IObjectReference instanceReference;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theDelegate">The component adapter to decorate</param>
        public CachingComponentAdapter(IComponentAdapter theDelegate) : this(theDelegate, new SimpleReference())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theDelegate">The component adapter to decorate</param>
        /// <param name="reference">Object to store the instance in. See <see cref="IObjectReference"/> for an explanation.</param>
        public CachingComponentAdapter(IComponentAdapter theDelegate, SimpleReference reference) : base(theDelegate)
        {
            instanceReference = reference;
        }

        /// <summary>
        /// Gets the component instance. Only one instance is created of the type
        /// </summary>
        /// <returns>a component instance</returns>
        /// <exception cref="PicoContainer.PicoInitializationException">if the component could not be instantiated.</exception>    
        /*public override object ComponentInstance
		{
			get
			{
				if (instanceReference.Get() == null)
				{
					instanceReference.Set(base.ComponentInstance);
				}
				return instanceReference.Get();
			}

		}*/
        public override object GetComponentInstance(IPicoContainer container)
        {
            if (instanceReference.Get() == null)
            {
                instanceReference.Set(base.GetComponentInstance(container));
            }
            return instanceReference.Get();
        }
    }
}