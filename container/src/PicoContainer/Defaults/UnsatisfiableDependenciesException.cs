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
using PicoContainer.Utils;

namespace PicoContainer.Defaults
{
    [Serializable]
    public class UnsatisfiableDependenciesException : PicoIntrospectionException
    {
        private readonly IList failedDependencies;
        private readonly IComponentAdapter instantiatingComponentAdapter;

        public UnsatisfiableDependenciesException(IComponentAdapter instantiatingComponentAdapter,
                                                  IList failedDependencies)
        {
            this.instantiatingComponentAdapter = instantiatingComponentAdapter;
            this.failedDependencies = failedDependencies;
        }

        public UnsatisfiableDependenciesException()
        {
        }

        public UnsatisfiableDependenciesException(Exception ex) : base(ex)
        {
        }

        public UnsatisfiableDependenciesException(string message) : base(message)
        {
        }

        public UnsatisfiableDependenciesException(string message, Exception ex) : base(message, ex)
        {
        }

        protected UnsatisfiableDependenciesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string Message
        {
            get
            {
                StringBuilder b =
                    new StringBuilder(instantiatingComponentAdapter.ComponentImplementation.Name).Append(
                        " doesn't have any satisfiable constructors. Unsatisfiable dependencies: ");
                b.Append(StringUtils.ArrayToString(((ArrayList) failedDependencies).ToArray()));
                return b.ToString();
            }
        }

        public IComponentAdapter UnsatisfiableComponentAdapter
        {
            get { return instantiatingComponentAdapter; }
        }

        public IList UnsatisfiableDependencies
        {
            get { return failedDependencies; }
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (!(o is UnsatisfiableDependenciesException)) return false;

            UnsatisfiableDependenciesException noSatisfiableConstructorsException =
                (UnsatisfiableDependenciesException) o;

            if (!instantiatingComponentAdapter.Equals(noSatisfiableConstructorsException.instantiatingComponentAdapter))
                return false;
            if (!failedDependencies.Equals(noSatisfiableConstructorsException.failedDependencies)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int result;
            result = instantiatingComponentAdapter.GetHashCode();
            result = 29*result + failedDependencies.GetHashCode();
            return result;
        }
    }
}