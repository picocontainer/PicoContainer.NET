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
	/// <summary>
	/// Summary description for CachingComponentAdapterFactory.
	/// </summary>
	[Serializable]
	public class CachingComponentAdapterFactory : DecoratingComponentAdapterFactory
	{
		public CachingComponentAdapterFactory(IComponentAdapterFactory theDelegate) : base(theDelegate)
		{
		}

		public override IComponentAdapter CreateComponentAdapter(object componentKey, Type componentImplementation, IParameter[] parameters)
		{
			return new CachingComponentAdapter(base.CreateComponentAdapter(componentKey, componentImplementation, parameters));
		}
	}
}