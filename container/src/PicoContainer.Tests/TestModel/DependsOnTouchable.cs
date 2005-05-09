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
using NUnit.Framework;

namespace PicoContainer.TestModel
{
	[Serializable]
	public class DependsOnTouchable
	{
		private ITouchable touchable;

		public DependsOnTouchable(ITouchable touchable)
		{
			Assert.IsNotNull(touchable, "Touchable cannot be passed in as null");
			touchable.Touch();
			this.touchable = touchable;
		}

		public Object getTouchable()
		{
			return touchable;
		}
	}
}