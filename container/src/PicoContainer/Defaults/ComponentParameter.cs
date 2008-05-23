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

namespace PicoContainer.Defaults
{
    /// <summary>
    /// A ComponentParameter should be used to pass in a particular component
    /// as argument to a different component's constructor. 
    /// <remarks>
    /// This is particularly useful in cases where several components of the 
    /// same type have been registered, but with a different key. Passing a 
    /// ComponentParameter as a parameter when registering a component will 
    /// give PicoContainer a hint about what other component to use in the 
    /// constructor.
    /// </remarks>
    /// </summary>
    [Serializable]
    public class ComponentParameter : BasicComponentParameter
    {
        /// <summary>
        /// Use <code>ARRAY</code> as IParameter for an Array that must have elements.
        /// </summary>
        public static readonly ComponentParameter ARRAY = new ComponentParameter(false);

        /// <summary>
        /// Use <code>ARRAY_ALLOW_EMPTY</code> as {@link Parameter}for an Array that may have no elements.
        /// </summary>
        public static readonly ComponentParameter ARRAY_ALLOW_EMPTY = new ComponentParameter(true);

        /// <summary>
        /// is an instance of ComponentParameter using the default constructor.
        /// </summary>
        public static readonly ComponentParameter DEFAULT = new ComponentParameter();

        private readonly IParameter collectionParameter;

        /**
		 * Expect a parameter matching a component of a specific key.
		 * 
		 * @param componentKey the key of the desired component
		 */

        public ComponentParameter(Object componentKey) : this(componentKey, null)
        {
        }

        /**
		 * Expect any scalar paramter of the appropriate type or an {@link java.lang.reflect.Array}.
		 */

        public ComponentParameter() : this(false)
        {
        }

        /**
		 * Expect any scalar paramter of the appropriate type or an {@link java.lang.reflect.Array}.
		 * Resolve the parameter even if no compoennt is of the array's component type.
		 * 
		 * @param emptyCollection <code>true</code> allows an Array to be empty
		 * @since 1.1
		 */

        public ComponentParameter(bool emptyCollection) :
            this(
            null, emptyCollection ? CollectionComponentParameter.ARRAY_ALLOW_EMPTY : CollectionComponentParameter.ARRAY)
        {
        }

        /**
		 * Expect any scalar paramter of the appropriate type or the collecting type
		 * {@link java.lang.reflect.Array},{@link java.util.Collection}or {@link java.util.Map}.
		 * The components in the collection will be of the specified type.
		 * 
		 * @param componentValueType the component's type (ignored for an Array)
		 * @param emptyCollection <code>true</code> allows the collection to be empty
		 * @since 1.1
		 */

        public ComponentParameter(Type componentValueType, bool emptyCollection) :
            this(null, new CollectionComponentParameter(componentValueType, emptyCollection))
        {
        }

        /**
		 * Expect any scalar paramter of the appropriate type or the collecting type
		 * {@link java.lang.reflect.Array},{@link java.util.Collection}or {@link java.util.Map}.
		 * The components in the collection will be of the specified type and their adapter's key
		 * must have a particular type.
		 * 
		 * @param componentKeyType the component adapter's key type
		 * @param componentValueType the component's type (ignored for an Array)
		 * @param emptyCollection <code>true</code> allows the collection to be empty
		 * @since 1.1
		 */

        public ComponentParameter(Type componentKeyType, Type componentValueType, bool emptyCollection)
            : this(null, new CollectionComponentParameter(componentKeyType, componentValueType, emptyCollection))
        {
        }

        private ComponentParameter(object componentKey, IParameter collectionParameter) : base(componentKey)
        {
            this.collectionParameter = collectionParameter;
        }


        public override object ResolveInstance(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            // type check is done in isResolvable
            Object result = base.ResolveInstance(container, adapter, expectedType);
            if (result == null && collectionParameter != null)
            {
                result = collectionParameter.ResolveInstance(container, adapter, expectedType);
            }
            return result;
        }

        public override bool IsResolvable(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            if (!base.IsResolvable(container, adapter, expectedType))
            {
                if (collectionParameter != null)
                {
                    return collectionParameter.IsResolvable(container, adapter, expectedType);
                }
                return false;
            }
            return true;
        }


        public override void Verify(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            try
            {
                base.Verify(container, adapter, expectedType);
            }
            catch (UnsatisfiableDependenciesException e)
            {
                if (collectionParameter != null)
                {
                    collectionParameter.Verify(container, adapter, expectedType);
                    return;
                }
                throw e;
            }
        }
    }
}