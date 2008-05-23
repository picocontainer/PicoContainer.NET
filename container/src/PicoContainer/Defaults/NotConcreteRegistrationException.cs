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
    [Serializable]
    public class NotConcreteRegistrationException : PicoRegistrationException
    {
        private Type componentImplementation;

        public NotConcreteRegistrationException(Type componentImplementation)
        {
            this.componentImplementation = componentImplementation;
        }

        public NotConcreteRegistrationException()
        {
        }

        public NotConcreteRegistrationException(Exception ex) : base(ex)
        {
        }

        public NotConcreteRegistrationException(string message) : base(message)
        {
        }

        public NotConcreteRegistrationException(string message, Exception ex) : base(message, ex)
        {
        }

        protected NotConcreteRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override String Message
        {
            get { return "Bad Access: '" + componentImplementation.Name + "' is not instantiable"; }
        }

        public Type ComponentImplementation
        {
            get { return componentImplementation; }
        }
    }
}