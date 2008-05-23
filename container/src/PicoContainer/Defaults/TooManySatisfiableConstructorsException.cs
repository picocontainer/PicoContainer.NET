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

namespace PicoContainer.Defaults
{
    [Serializable]
    public class TooManySatisfiableConstructorsException : PicoIntrospectionException
    {
        private ICollection constructors;
        private Type forClass;

        public TooManySatisfiableConstructorsException(Type forClass, ICollection constructors)
        {
            this.forClass = forClass;
            this.constructors = constructors;
        }

        public TooManySatisfiableConstructorsException()
        {
        }

        public TooManySatisfiableConstructorsException(Exception ex) : base(ex)
        {
        }

        public TooManySatisfiableConstructorsException(string message) : base(message)
        {
        }

        public TooManySatisfiableConstructorsException(string message, Exception ex) : base(message, ex)
        {
        }

        protected TooManySatisfiableConstructorsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type ForImplementationClass
        {
            get { return forClass; }
        }

        public override String Message
        {
            get { return "Too many satisfiable constructors:" + constructors.ToString(); }
        }

        public ICollection Constructors
        {
            get { return constructors; }
        }
    }
}