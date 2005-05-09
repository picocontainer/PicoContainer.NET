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
	/// The PicoInvocationTargetInitializationException is thrown when an error occurs while creating an 
	/// instance of a registered <see cref="PicoContainer.IComponentAdapter"/>.
	/// </summary>
	/// <remarks>
	/// </remarks>
	[Serializable]
	public class PicoInvocationTargetInitializationException : PicoInstantiationException
	{
		public PicoInvocationTargetInitializationException(Exception cause) 
			: base("InvocationTargetException: " + cause.GetType().FullName + " " + cause.Message
			, cause)
		{
		}
	}
}