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

namespace PicoContainer
{
	/// <summary>This is the core interface for PicoContainer. It only has accessor methods.</summary>
	/// <remarks>In order to register components in a PicoContainer, use a <see cref="PicoContainer.IMutablePicoContainer"/>,
	/// such as <see cref="Defaults.DefaultPicoContainer"/>.</remarks>
	public interface IPicoContainer : IStartable, IDisposable
	{
		/// <summary>
		/// Retrieve a component instance registered with a specific key.
		/// </summary>
		/// <remarks>
		/// If a component cannot be found in this container,
		/// the parent container (if one exists) will be searched.
		/// </remarks>
		/// <param name="componentKey">the key the component was registered with.</param>
		/// <returns>an instantiated component, , or <example>null</example> if no component has been registered for the specified
		/// key.</returns>
		object GetComponentInstance(object componentKey);

		/// <summary>
		/// Finds a component instance matching the type, looking in parent if
		/// not found in self (unless parent is null).
		/// </summary>
		/// <param name="componentType">type of the compontent</param>
		/// <returns>the adapter matching the type</returns>
		object GetComponentInstanceOfType(Type componentType);

		/// <summary>
		/// Gets all the registered component instances in the container (not including 
		/// those in the parent container).</summary>
		/// <remarks>The components are returned in their order of instantiation, which
		/// depends on the dependency order between components.</remarks>
		/// <returns>all the components</returns>
		IList ComponentInstances { get; }

		/// <summary>
		/// Returns a List of components of a certain componentType. The list is ordered by instantiation order,
		/// starting with the components instantiated first at the beginning.
		/// </summary>
		/// <param name="componentType">the searched type.</param>
		/// <returns>List of components</returns>
		IList GetComponentInstancesOfType(Type componentType);

		/// <summary>
		/// Get the parent of this container, or <example>null</example> if this container does not have a parent.
		/// </summary>
		IPicoContainer Parent { get; }

		/// <summary>
		/// Finds a ComponentAdapter matching the key
		/// </summary>
		/// <remarks>If a component adapter cannot be found in this
		/// container, the parent container (if one exists) will be searched.</remarks>
		/// <param name="componentKey">key of the component</param>
		/// <returns>the component adapter associated with this key, or <example>null</example> if no component has been registered
		/// for the specified key.</returns>
		IComponentAdapter GetComponentAdapter(object componentKey);

		/// <summary>
		/// Finds a ComponentAdapter matching the type. 
		/// </summary>
		/// <remarks>If a component adapter cannot be found in this
		/// container, the parent container (if one exists) will be searched.</remarks>
		/// <param name="componentType">type of the component.</param>
		/// <returns>the component adapter associated with this class, or <example>null</example> if no component has been
		/// registered for the specified key.</returns>
		IComponentAdapter GetComponentAdapterOfType(Type componentType);


		/// <summary>
		/// Retrieve all the component adapters inside this container. 
		/// </summary>
		/// <remarks>The component adapters from the parent container are
		/// not returned.
		/// <see cref="GetComponentAdaptersOfType(Type)"/> a variant of this method which returns the component adapters inside this
		/// container that are associated with the specified type.
		/// </remarks>
		/// <returns>List of <see cref="PicoContainer.IComponentAdapter"/> all the {@link ComponentAdapter}s inside this container. 
		/// The collection will be readonly.</returns>
		IList ComponentAdapters { get; }

		/// <summary>
		/// Retrieve all the component adapters inside this container. 
		/// </summary>
		/// <remarks>The component adapters from the parent container are
		/// not returned.
		/// </remarks>
		/// <param name="componentType">type of the component</param>
		/// <returns>a collection containing all the {@link ComponentAdapter}s inside this container. The collection is readonly.
		/// </returns>
		IList GetComponentAdaptersOfType(Type componentType);

		/// <summary>
		/// Verifies that the dependencies for all the registered components can be satisfied
		/// </summary>
		/// <remarks>None of the components are instantiated during the verification process.</remarks>
		/// <exception cref="PicoVerificationException">if there are unsatisifiable dependencies.</exception>
		void Verify();

		/// <summary>
		/// Callback method from the implementation to keep track of the instantiation
		/// order. 
		/// </summary>
		/// <remarks>This method is not intended to be called explicitly by clients of the API!</remarks>
		/// <param name="componentAdapter">the adapter</param>
		void AddOrderedComponentAdapter(IComponentAdapter componentAdapter);
	}
}