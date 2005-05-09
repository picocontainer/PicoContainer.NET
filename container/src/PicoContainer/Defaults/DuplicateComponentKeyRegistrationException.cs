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
using System.Runtime.Serialization;
using PicoContainer;

namespace PicoContainer.Defaults
{
	[Serializable]
	public class DuplicateComponentKeyRegistrationException : PicoRegistrationException
	{
		private object key;

		public DuplicateComponentKeyRegistrationException(object key)
		{
			this.key = key;
		}

		public DuplicateComponentKeyRegistrationException()
		{
		}

		public DuplicateComponentKeyRegistrationException(Exception ex) : base(ex)
		{
		}

		public DuplicateComponentKeyRegistrationException(string message) : base(message)
		{
		}

		public DuplicateComponentKeyRegistrationException(string message, Exception ex) : base(message, ex)
		{
		}

		protected DuplicateComponentKeyRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public object DuplicateKey
		{
			get { return key; }
		}

		public override String Message
		{
			get { return "Key " + key.GetType().Name + " duplicated"; }
		}
	}
}