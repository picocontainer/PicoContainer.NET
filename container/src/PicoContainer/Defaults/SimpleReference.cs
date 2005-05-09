using System;
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
	/// <summary>
	/// Summary description for SimpleReference.
	/// </summary>
	[Serializable]
	public class SimpleReference : IObjectReference
	{
		private object instance;

		public object Get()
		{
			return instance;
		}

		public void Set(object item)
		{
			this.instance = item;
		}
	}
}