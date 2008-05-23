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
    /// Exception that is thrown when there is a problem initializing the container or some other
    /// part of the PicoContainer api, for example, when a cyclic dependency between components occurs.
    /// </summary>
    [Serializable]
    public class PicoInitializationException : PicoException
    {
        /**
	 * Construct a new exception with no cause and no detail message. Note modern JVMs may still track the exception
	 * that caused this one.
	 */

        protected PicoInitializationException()
        {
        }

        /**
		 * Construct a new exception with no cause and the specified detail message.  Note modern JVMs may still track the
		 * exception that caused this one.
		 *
		 * @param message the message detailing the exception.
		 */

        public PicoInitializationException(String message) : base(message)
        {
        }

        /**
	 * Construct a new exception with the specified cause and no detail message.
	 * 
	 * @param cause the exception that caused this one.
	 */

        public PicoInitializationException(Exception cause) : base(cause)
        {
        }

        /**
	 * Construct a new exception with the specified cause and the specified detail message.
	 *
	 * @param message the message detailing the exception.
	 * @param cause   the exception that caused this one.
	 */

        public PicoInitializationException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}