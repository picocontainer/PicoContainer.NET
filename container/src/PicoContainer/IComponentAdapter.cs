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
	/// A component adapter is responsible for providing a specific component instance.
	/// </summary>
	/// <remarks>
	/// An instance of an implementation of this interface is used in a <see cref="IPicoContainer"/>
	/// for every registered component or instance. Each ComponentAdapter instance has to 
	/// support unique key for a single PicoContainer. The key itself is either a class type
	/// (normally an interface) or an identifier.
	/// See <see cref="IMutablePicoContainer"/>an extension of the PicoContainer interface which 
	/// allows you to modify the contents of the container.</remarks>
	/// 
	public interface IComponentAdapter
	{
		/// <summary>
		/// Retrieve the key associated with the component.
		/// </summary>
		object ComponentKey { get; }

		/// <summary>
		/// Retrieve the implementing Type of the component.
		/// </summary>
		Type ComponentImplementation { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		object GetComponentInstance(IPicoContainer container);

		/// 
		/// <summary>
		///  Property containing the container in which this instance is registered, called by the container upon registration
		/// </summary>
		IPicoContainer Container { get; set; }

		void Verify(IPicoContainer container);
	}
}