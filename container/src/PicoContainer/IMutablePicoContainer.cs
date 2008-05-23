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
    /// This is the core interface used for registration of components with a container. 
    /// </summary>
    /// <remarks>It is possible to 
    /// register a (<see cref="IMutablePicoContainer.RegisterComponentImplementation(object, Type)"/>)  an 
    /// implementation class or <see cref="IMutablePicoContainer.RegisterComponent(IComponentAdapter)"/> 
    /// a ComponentAdapter.</remarks>
    public interface IMutablePicoContainer : IPicoContainer
    {
        /// <summary>Registers a component.</summary>
        /// <param name="componentKey">a key that identifies the compoent. Must be unique within the container.<remarks>The type of the key object has no semantic significance unless explicitly specified
        /// in the implementing container.</remarks></param>
        /// <param name="componentImplementation">the concrete component type</param>
        /// <returns>the associated ComponentAdapter.</returns>
        /// <exception cref="PicoRegistrationException">if the registration fails</exception>
        /// <see cref="RegisterComponentImplementation(Object,Type,IParameter[])"/> a variant of this method that allows more control
        /// over the parameters passed into the componentImplementation constructor when constructing an instance. 
        IComponentAdapter RegisterComponentImplementation(object componentKey, Type componentImplementation);

        /// <summary>
        /// Registers a component.</summary>
        /// <param name="componentKey">a key that identifies the compoent. Must be unique within the container.
        /// <remarks>The type of the key object has no semantic significance unless explicitly specified
        /// in the implementing container.</remarks></param>
        /// <param name="componentImplementation">the component's implementation type.
        /// <remarks>This must be a concrete class (ie, a class that can be instantiated).</remarks></param>
        /// <param name="parameters">an array of parameters that gives the container hints about what arguments
        /// to pass to the constructor when it is instantiated.
        /// <remarks>Container implementations may ignore one or more of these hints.</remarks></param>
        /// <returns>the associated ComponentAdapter.</returns>
        /// <exception cref="PicoContainer.PicoRegistrationException">if the registration fails</exception>
        IComponentAdapter RegisterComponentImplementation(object componentKey, Type componentImplementation,
                                                          IParameter[] parameters);

        /// <summary>
        /// Registers a component using the ComponentImplementation type as key.
        /// </summary>
        /// <remarks>Calling this method is equivalent to calling <example>RegisterComponentImplementation(componentImplementation, componentImplementation)</example></remarks>
        /// <param name="componentImplementation">the concrete component type</param>
        /// <returns>the associated ComponentAdapter.</returns>
        /// <exception cref="PicoContainer.PicoRegistrationException">if the registration fails</exception>
        IComponentAdapter RegisterComponentImplementation(Type componentImplementation);

        /// <summary>
        /// Registers an arbitrary object, using it's class as a key.
        /// </summary>
        /// <param name="componentInstance">the object to register</param>
        /// <returns>the associated ComponentAdapter.</returns>
        /// <exception cref="PicoContainer.PicoRegistrationException">if the registration fails</exception>
        IComponentAdapter RegisterComponentInstance(object componentInstance);

        /// <summary>
        /// Registers an arbitrary object as a component in the container.
        /// </summary>
        /// <remarks>This is handy when other components in the same container have dependencies on this
        /// kind of object, but where letting the container manage and instantiate it is impossible.<br/>
        /// Beware that too much use of this method is an antipattern.
        /// <a href="http://docs.codehaus.org/display/PICO/Instance+Registration">antipattern</a>.
        /// </remarks>
        /// <param name="componentKey">a key that identifies the compoent. Must be unique within the container    
        /// <remarks>The type of the key object has no semantic significance unless explicitly specified
        /// in the implementing container.</remarks></param>
        /// <param name="componentInstance">an arbitrary object.</param>
        /// <returns>the associated ComponentAdapter.</returns>
        /// <exception cref="PicoContainer.PicoRegistrationException">if the registration fails</exception>
        IComponentAdapter RegisterComponentInstance(object componentKey, object componentInstance);

        /// <summary>
        /// Registers a component via an <see cref="PicoContainer.IComponentAdapter"/>. 
        /// </summary>
        /// <remarks>Use this if you need fine grained control over what ComponentAdapter
        /// to use for a specific component.</remarks>
        /// <param name="componentAdapter">the adapter to register</param>
        /// <returns>the passed in ComponentAdapter.</returns>
        /// <exception cref="PicoContainer.PicoRegistrationException">if the registration fails</exception>    
        IComponentAdapter RegisterComponent(IComponentAdapter componentAdapter);

        /// <summary>
        /// Unregisters a component.</summary>
        /// <param name="componentKey">key of the component to unregister.</param>
        /// <returns>the associated ComponentAdapter.</returns>
        IComponentAdapter UnregisterComponent(object componentKey);

        /// <summary>
        /// Unregisters a component using the instance of the component.</summary>
        /// <param name="componentInstance">instance of the component to unregister.</param>
        /// <returns>the associated ComponentAdapter.</returns>
        IComponentAdapter UnregisterComponentByInstance(object componentInstance);

        /// <summary>
        /// Make a child container, using the same implementation of MutablePicoContainer as the parent.
        /// It will have a reference to this as parent.  This will list the resulting MPC as a child.
        /// Lifecycle events will be cascaded from parent to child
        /// </summary>
        /// <returns>the new child container.</returns>
        IMutablePicoContainer MakeChildContainer();

        /// <summary>
        /// Add a child container. This action will list the the 'child' as exactly that in the parents scope.
        /// It will not change the child's view of a parent.  That is determined by the constructor arguments of the child
        /// itself. Lifecycle events will be cascaded from parent to child
        /// as a consequence of calling this method.
        /// </summary>
        /// <param name="child">the child container</param>
        /// <returns>true if the child container was not already in.</returns>
        bool AddChildContainer(IPicoContainer child);

        /// <summary>
        /// Remove a child container from this container. It will not change the child's view of a parent.
        /// Lifecycle event will no longer be cascaded from the parent to the child.
        /// </summary>
        /// <param name="child">the child container</param>
        /// <returns>true if the child container has been removed.</returns>
        bool RemoveChildContainer(IPicoContainer child);
    }
}