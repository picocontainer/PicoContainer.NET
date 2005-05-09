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
	public class DecoratingComponentAdapterFactory : IComponentAdapterFactory
	{
		private readonly IComponentAdapterFactory theDelegate;

		public DecoratingComponentAdapterFactory(IComponentAdapterFactory theDelegate)
		{
			this.theDelegate = theDelegate;
		}

		public virtual IComponentAdapter CreateComponentAdapter(object componentKey, Type componentImplementation, IParameter[] parameters)
		{
			return theDelegate.CreateComponentAdapter(componentKey, componentImplementation, parameters);
		}
	}
}