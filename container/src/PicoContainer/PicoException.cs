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

namespace PicoContainer
{
	/// <summary>
	/// Superclass for all Exceptions in PicoContainer. 
	/// </summary>
	/// <remarks>
	/// You can use this if you want to catch all exceptions thrown by
	/// PicoContainer. Be aware that some parts of the PicoContainer API will also throw <see cref="NullReferenceException"/> when
	/// <example>null</example> values are provided for method arguments, and this is not allowed.
	/// </remarks>
	[Serializable]
	public class PicoException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PicoException"/> class.
		/// </summary>
		/// <remarks>
		/// This constructor initializes the Message property of the new instance to a system-supplied message 
		/// that describes the error, such as "PicoContainer caused an exception." 
		/// </remarks>
		public PicoException() : base("PicoContainer caused an exception.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PicoContainer.PicoException"/> class with a specified error message.
		/// </summary>
		/// <remarks>
		/// This constructor initializes the Message property of the new instance using the message parameter.
		/// </remarks>
		/// <param name="ex">The exception that is the cause of the current exception. 
		/// If the innerException parameter is not a null reference (Nothing in Visual Basic), 
		/// the current exception is raised in a catch block that handles the inner exception.</param>
		public PicoException(Exception ex) : base("PicoContainer caused an exception.", ex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PicoContainer.PicoException"/> class with a 
		/// reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <remarks>
		/// This constructor initializes the Message property of the new instance to the Message property of the 
		/// passed in exception. 
		/// </remarks>
		/// <param name="message">The message that describes the error.</param>
		public PicoException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PicoContainer.PicoException"/> class with a specified error message 
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <remarks>
		/// An exception that is thrown as a direct result of a previous exception should include a reference to the previous 
		/// exception in the InnerException property. 
		/// The InnerException property returns the same value that is passed into the constructor, or a null reference 
		/// (Nothing in Visual Basic) if the InnerException property does not supply the inner exception value to the constructor.
		/// </remarks>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="ex">The exception that caused the error</param>
		public PicoException(string message, Exception ex) : base(message, ex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PicoContainer.PicoException"/> class with serialized data.
		/// </summary>
		/// <remarks>
		/// This constructor is called during deserialization to reconstitute the exception object transmitted over a stream.
		/// </remarks>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
		/// object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual 
		/// information about the source or destination. </param>
		protected PicoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}