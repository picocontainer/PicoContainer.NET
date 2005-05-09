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
	/// Decorates a Component adapter, used for combining the functionality of multiple IComponentAdapters
	/// </summary>
	[Serializable]
	public class DecoratingComponentAdapter : IComponentAdapter
	{
		private IComponentAdapter theDelegate;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="theDelegate">The component adapter to decorate</param>
		public DecoratingComponentAdapter(IComponentAdapter theDelegate)
		{
			this.theDelegate = theDelegate;
		}

		/// <summary>
		/// Returns the component's key
		/// </summary>
		virtual public object ComponentKey
		{
			get { return theDelegate.ComponentKey; }
		}

		/// <summary>
		/// Returns the component's implementing type
		/// </summary>
		virtual public Type ComponentImplementation
		{
			get { return theDelegate.ComponentImplementation; }
		}

		public virtual object GetComponentInstance(IPicoContainer container)
		{
			return theDelegate.GetComponentInstance(container);
		}

		/// <summary>
		/// Verify that all dependencies for this adapter can be satisifed.
		/// </summary>
		/// <exception cref="PicoContainer.PicoIntrospectionException">if the verification failed</exception>
		public virtual void Verify(IPicoContainer container)
		{
			theDelegate.Verify(container);
		}

		/// <summary>
		/// The delegate decorated by this adapter
		/// </summary>
		public virtual IComponentAdapter Delegate
		{
			get { return theDelegate; }
		}

		/// 
		/// <summary>
		///  Property containing the container in which this instance is registered, called by the container upon registration
		/// </summary>
		public virtual IPicoContainer Container
		{
			get { return theDelegate.Container; }
			set { theDelegate.Container = value; }
		}
	}
}