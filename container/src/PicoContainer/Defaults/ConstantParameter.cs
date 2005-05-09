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
	/**
 *
 * @author Jon Tirs&eacute;n
 * @author Aslak Helles&oslash;y
 * @author J&ouml;rg Schaible
 * @version $Revision$
 */

	/// <summary>
	/// A ConstantParameter should be used to pass in "constant" arguments
	/// to constructors. 
	/// <remarks>This includes Strings, Integers or
	/// any other object that is not registered in the container.</remarks>
	/// </summary>
	[Serializable]
	public class ConstantParameter : IParameter
	{
		private readonly object constantValue;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="constantValue">the value</param>
		public ConstantParameter(object constantValue)
		{
			this.constantValue = constantValue;
		}

		public virtual Object ResolveInstance(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
		{
			return constantValue;
		}

		public virtual bool IsResolvable(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
		{
			try
			{
				Verify(container, adapter, expectedType);
				return true;
			}
			catch(PicoIntrospectionException)
			{
				return false;
			}
		}

		public void Verify(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
		{
			if (!expectedType.IsInstanceOfType(constantValue))
			{
				throw new PicoIntrospectionException(expectedType.FullName 
					+ " is not assignable from "
					+ constantValue.GetType().FullName);
			}
		}
	}
}