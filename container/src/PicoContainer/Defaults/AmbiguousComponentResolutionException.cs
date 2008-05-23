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
using System.Text;

namespace PicoContainer.Defaults
{
    /// <summary>
    /// The PicoIntrospectionException is thrown when the initialization could be done with more than one
    /// component.
    /// </summary>
    [Serializable]
    public class AmbiguousComponentResolutionException : PicoIntrospectionException
    {
        private readonly object[] ambiguousComponentKeys;
        private readonly Type ambiguousType;
        private Type component;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ambiguousType">The type that could be resolved with more than one component.</param>
        /// <param name="componentKeys">The keys of the components that where resolved</param>
        public AmbiguousComponentResolutionException(Type ambiguousType, object[] componentKeys)
        {
            this.ambiguousType = ambiguousType;
            ambiguousComponentKeys = new Type[componentKeys.Length];
            for (int i = 0; i < componentKeys.Length; i++)
            {
                ambiguousComponentKeys[i] = componentKeys[i];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousComponentResolutionException"/> class with serialized data.
        /// </summary>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute the exception object transmitted over a stream.
        /// </remarks>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination. </param>
        protected AmbiguousComponentResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Returns a customized message showing what type resolved to multiple keys.
        /// </summary>
        public override String Message
        {
            get
            {
                StringBuilder msg = new StringBuilder(component.ToString())
                    .Append(" has ambiguous dependency on ")
                    .Append(ambiguousType)
                    .Append(", ")
                    .Append("resolves to multiple classes: [");

                for (int i = 0; i < AmbiguousComponentKeys.Length; i++)
                {
                    if (i != 0)
                    {
                        msg.Append(", ");
                    }

                    msg.Append(AmbiguousComponentKeys[i]);
                }

                msg.Append("]");
                return msg.ToString();
            }
        }

        /// <summary>
        /// The keys that resolved the dependency.
        /// </summary>
        public object[] AmbiguousComponentKeys
        {
            get { return ambiguousComponentKeys; }
        }

        public Type Component
        {
            set { component = value; }
        }
    }
}