/***********************************************************************  ******
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
    /// This class represents an argument to a constructor. </summary>
    /// <remarks>It can be used to
    /// have finer control over what arguments are passed to a particular constructor.</remarks>
    /// <see cref="IMutablePicoContainer.RegisterComponentImplementation(object,Type,IParameter[])"/> a method on the 
    /// <see cref="IMutablePicoContainer"/> interface which allows passing in of an array of <example>IParameter</example>s.
    /// <see cref="Defaults.ComponentParameter"/> an implementation of this interface that allows you to specify the
    /// key used for resolving the parameter.
    /// <see cref="Defaults.ConstantParameter"/> an implementation of this interface that allows you to specify a
    /// constant that will be used for resolving the parameter.
    public interface IParameter
    {
        /// <summary>
        /// Retrieve the object from the Parameter that statisfies the expected type.
        /// </summary>
        /// <param name="container">the container from which dependencies are resolved.</param>
        /// <param name="adapter">the ComponentAdapter that is asking for the instance</param>
        /// <param name="expectedType">the type that the returned instance needs to match.</param>
        /// <returns>the instance or <code>null</code> if no suitable instance can be found.</returns>
        object ResolveInstance(IPicoContainer container, IComponentAdapter adapter, Type expectedType);

        /// <summary>
        /// Check if the Parameter can statisfy the expected type using the container.
        /// </summary>
        /// <param name="container">the container from which dependencies are resolved.</param>
        /// <param name="adapter">the ComponentAdapter that is asking for the instance</param>
        /// <param name="expectedType">the type that the returned instance needs to match.</param>
        /// <returns>true if the component parameter can be resolved.</returns>
        bool IsResolvable(IPicoContainer container, IComponentAdapter adapter, Type expectedType);

        /// <summary>
        /// Verify that the Parameter can statisfied the expected type using the container
        /// </summary>
        /// <param name="container">the container from which dependencies are resolved.</param>
        /// <param name="adapter">the ComponentAdapter that is asking for the instance</param>
        /// <param name="expectedType">the type that the returned instance needs to match.</param>
        void Verify(IPicoContainer container, IComponentAdapter adapter, Type expectedType);
    }
}