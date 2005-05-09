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

namespace PicoContainer.TestModel
{
	[Serializable]
	public class WashableTouchable : ITouchable, IWashable
	{
		public WashableTouchable()
		{
		}

		public bool WasTouched
		{
			get { return true; }
		}

		public void Touch()
		{
		}

		public void Wash()
		{
		}
	}
}