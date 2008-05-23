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

namespace PicoContainer.Defaults
{
    /// <summary>
    /// An exception thrown when a type could not be assigned.
    /// </summary>
    /// TODO hello
    [Serializable]
    public class AssignabilityRegistrationException : PicoRegistrationException
    {
        private readonly Type type;
        private readonly Type typeToAssign;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignabilityRegistrationException"/> class with serialized data.
        /// </summary>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute the exception object transmitted over a stream.
        /// </remarks>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination. </param>
        protected AssignabilityRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new ins
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeToAssign"></param>
        public AssignabilityRegistrationException(Type type, Type typeToAssign)
        {
            this.type = type;
            this.typeToAssign = typeToAssign;
        }

        /// <summary>
        /// Returns a customized message containing the name of the types.
        /// </summary>
        public override String Message
        {
            get { return "The type:" + type.Name + "  was not assignable from the class " + typeToAssign.Name; }
        }
    }
}