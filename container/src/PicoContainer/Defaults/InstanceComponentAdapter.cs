using System;
using PicoContainer;
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

namespace PicoContainer.Defaults
{
	[Serializable]
	public class InstanceComponentAdapter : AbstractComponentAdapter
	{
		private object componentInstance;

		public InstanceComponentAdapter(object componentKey, object componentInstance) : base(componentKey, componentInstance.GetType())
		{
			this.componentInstance = componentInstance;
		}

		public override object GetComponentInstance(IPicoContainer container)
		{
			return componentInstance;
		}

		public override void Verify(IPicoContainer container)
		{
		}
	}
}