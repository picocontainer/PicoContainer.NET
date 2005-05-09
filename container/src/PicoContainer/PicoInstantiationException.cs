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

namespace PicoContainer
{
	/// <summary>
	/// Exception that is thrown when there is a problem creating an instance of a container or some
	/// other part of the PicoContainer api, for example, when an invocation through the reflection api fails.  
	/// </summary>
	[Serializable]
	public class PicoInstantiationException : PicoInitializationException
	{
		protected PicoInstantiationException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}