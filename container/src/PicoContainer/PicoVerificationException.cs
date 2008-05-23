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
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace PicoContainer
{
    /// <summary>
    /// Exception that is thrown when there is a problem with the internal state of the container or
    /// another part of the PicoContainer API, for example when a needed dependency cannot be resolved.
    /// </summary>
    [Serializable]
    public class PicoVerificationException : PicoException
    {
        private readonly IList nestedExceptions = new ArrayList();

        /// <summary>
        /// Initializes a new instance of the <see cref="PicoContainer.PicoException"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the Message property of the new instance to a system-supplied message 
        /// that describes the error, such as "Verification of the conatainer failed." 
        /// </remarks>
        public PicoVerificationException() : base("Verification of the conatainer failed.")
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
        public PicoVerificationException(Exception ex) : base(ex.Message, ex)
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
        public PicoVerificationException(string message) : base(message)
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
        public PicoVerificationException(string message, Exception ex) : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PicoContainer.PicoException"/> class with a list of exceptions 
        /// that where thrown during the verification.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the Message property of the new instance to a system-supplied message 
        /// that describes the error, such as "Verification of the conatainer failed." 
        /// </remarks>
        /// <param name="nestedExceptions">a list of exceptions occurred during verification.</param>
        public PicoVerificationException(IList nestedExceptions)
        {
            this.nestedExceptions = nestedExceptions;
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
        protected PicoVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Readonly property containing a list of exceptions thrown during the verification of the container.
        /// </summary>
        /// <remarks>
        /// Can be null (Nothing in VisualBasic).
        /// </remarks>
        public IList NestedExceptions
        {
            get { return nestedExceptions; }
        }

        /// <summary>
        /// Returns a string containing all errors occured during verification
        /// </summary>
        public override string Message
        {
            get
            {
                StringBuilder buff = new StringBuilder(base.Message);
                foreach (Exception ex in nestedExceptions)
                {
                    buff.Append(ex.Message);
                    buff.Append("\n");
                }
                return buff.ToString();
            }
        }
    }
}