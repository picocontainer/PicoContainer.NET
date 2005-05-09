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
using PicoContainer;

namespace PicoContainer.Defaults
{
	[Serializable]
	public class ConstructorInjectionComponentAdapterFactory : IComponentAdapterFactory
	{
		private bool allowNonPublicClasses;
		private IComponentMonitor componentMonitor;

		public ConstructorInjectionComponentAdapterFactory(bool allowNonPublicClasses, IComponentMonitor componentMonitor) 
		{
			this.allowNonPublicClasses = allowNonPublicClasses;
			this.componentMonitor = componentMonitor;
		}

		public ConstructorInjectionComponentAdapterFactory(bool allowNonPublicClasses) 
			: this(allowNonPublicClasses, new NullComponentMonitor())
		{
		}

		public ConstructorInjectionComponentAdapterFactory() : this(false)
		{
		}

		public IComponentAdapter CreateComponentAdapter(object componentKey,
		                                                Type componentImplementation,
		                                                IParameter[] parameters)
		{
			return new ConstructorInjectionComponentAdapter(componentKey, componentImplementation, parameters, allowNonPublicClasses, componentMonitor);
		}
	}
}