using System;
using System.Collections;

namespace PicoContainer.Defaults
{
    [Serializable]
    public class CollectionComponentParameter : IParameter
    {
        /// <summary>
        /// Use <code>ARRAY</code> as Parameterfor an Array that must have elements.
        /// </summary>
        public static readonly CollectionComponentParameter ARRAY = new CollectionComponentParameter();

        /// <summary>
        /// Use <code>ARRAY_ALLOW_EMPTY</code> as {@link Parameter}for an Array that may have no elements.
        /// </summary>
        public static readonly CollectionComponentParameter ARRAY_ALLOW_EMPTY = new CollectionComponentParameter(true);

        private readonly Type componentKeyType;
        private readonly Type componentValueType;
        private readonly bool emptyCollection;

        /// <summary>
        /// Expect an {@link Array}of an appropriate type as parameter. At least one component of
        /// the array's component type must exist.
        /// </summary>
        public CollectionComponentParameter() : this(false)
        {
        }

        /// <summary>
        /// Expect an {@link Array}of an appropriate type as parameter.
        /// </summary>
        /// <param name="emptyCollection"><code>true</code> if an empty array also is a valid dependency resolution.</param>
        public CollectionComponentParameter(bool emptyCollection) : this(typeof (void), emptyCollection)
        {
        }

        /// <summary>
        /// Expect any of the collection types {@link Array},{@link Collection}or {@link Map}as parameter.
        /// </summary>
        /// <param name="componentValueType">type of the components (ignored in case of an Array)</param>
        /// <param name="emptyCollection"><code>true</code> if an empty collection resolves the dependency.</param>
        public CollectionComponentParameter(Type componentValueType, bool emptyCollection)
            : this(typeof (object), componentValueType, emptyCollection)
        {
        }

        /// <summary>
        /// Expect any of the collection types {@link Array},{@link Collection}or {@link Map}as parameter.
        /// </summary>
        /// <param name="componentKeyType">the type of the component's key</param>
        /// <param name="componentValueType">the type of the components (ignored in case of an Array)</param>
        /// <param name="emptyCollection"><code>true</code> if an empty collection resolves the dependency.</param>
        public CollectionComponentParameter(Type componentKeyType, Type componentValueType, bool emptyCollection)
        {
            this.emptyCollection = emptyCollection;
            this.componentKeyType = componentKeyType;
            this.componentValueType = componentValueType;
        }

        #region IParameter Members

        /// <summary>
        /// Resolve the parameter for the expected type. The method will return <code>null</code>
        /// If the expected type is not one of the collection types {@link Array},
        /// {@link Collection}or {@link Map}. An empty collection is only a valid resolution, if
        /// the <code>emptyCollection</code> flag was set.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="adapter"></param>
        /// <param name="expectedType"></param>
        /// <returns>the instance of the collection type or <code>null</code></returns>
        public Object ResolveInstance(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            // type check is done in isResolvable
            Object result = null;
            Type collectionType = GetCollectionType(expectedType);
            if (collectionType != null)
            {
                IDictionary dictionary =
                    GetMatchingComponentAdapters(container, adapter, componentKeyType, GetValueType(expectedType));
                if (typeof (Array).IsAssignableFrom(collectionType))
                {
                    result = GetArrayInstance(container, expectedType, dictionary);
                }
                else if (typeof (IDictionary).IsAssignableFrom(collectionType))
                {
                    result = GetDictionaryInstance(container, expectedType, dictionary);
                }
                else if (typeof (ICollection).IsAssignableFrom(collectionType))
                {
                    result = GetCollectionInstance(container, expectedType, dictionary);
                }
                else
                {
                    throw new PicoIntrospectionException(expectedType.Name + " is not a collective type");
                }
            }
            return result;
        }

        /// <summary>
        /// Check for a successful dependency resolution of the parameter for the expected type. The
        /// dependency can only be satisfied if the expected type is one of the collection types
        /// {@link Array},{@link Collection}or {@link Map}. An empty collection is only a valid
        /// resolution, if the <code>emptyCollection</code> flag was set.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="adapter"></param>
        /// <param name="expectedType"></param>
        /// <returns><code>true</code> if matching components were found or an empty collective type is allowed</returns>
        public bool IsResolvable(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            Type collectionType = GetCollectionType(expectedType);
            Type valueType = GetValueType(expectedType);

            return
                collectionType != null &&
                (emptyCollection ||
                 GetMatchingComponentAdapters(container, adapter, componentKeyType, valueType).Count > 0);
        }

        /// <summary>
        /// Verify a successful dependency resolution of the parameter for the expected type. The
        /// method will only return if the expected type is one of the collection types {@link Array},
        /// {@link Collection}or {@link Map}. An empty collection is only a valid resolution, if
        /// the <code>emptyCollection</code> flag was set.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="adapter"></param>
        /// <param name="expectedType"></param>
        public void Verify(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            Type collectionType = GetCollectionType(expectedType);
            if (collectionType != null)
            {
                Type valueType = GetValueType(expectedType);
                ICollection componentAdapters =
                    GetMatchingComponentAdapters(container, adapter, componentKeyType, valueType).Values;
                if (componentAdapters.Count == 0)
                {
                    if (!emptyCollection)
                    {
                        throw new PicoIntrospectionException(expectedType.FullName
                                                             + " not resolvable, no components of type "
                                                             + GetValueType(expectedType).FullName
                                                             + " available");
                    }
                }
                else
                {
                    foreach (IComponentAdapter componentAdapter in componentAdapters)
                    {
                        componentAdapter.Verify(container);
                    }
                }
            }
            else
            {
                throw new PicoIntrospectionException(expectedType.Name + " is not a collective type");
            }
            return;
        }

        #endregion

        /// <summary>
        /// Evaluate whether the given component adapter will be part of the collective type.
        /// </summary>
        /// <param name="adapter">a <code>ComponentAdapter</code> value</param>
        /// <returns><code>true</code> if the adapter takes part</returns>
        protected virtual bool Evaluate(IComponentAdapter adapter)
        {
            return adapter != null; // use parameter, prevent compiler warning
        }

        /// <summary>
        /// Collect the matching ComponentAdapter instances. 
        /// </summary>
        /// <param name="container">container container to use for dependency resolution</param>
        /// <param name="adapter">ComponentAdapter to exclude</param>
        /// <param name="keyType">the compatible type of the key</param>
        /// <param name="valueType">the compatible type of the component</param>
        /// <returns>IDictionary with the ComponentAdapter instances and their component keys as map key.</returns>
        protected IDictionary GetMatchingComponentAdapters(IPicoContainer container,
                                                           IComponentAdapter adapter,
                                                           Type keyType,
                                                           Type valueType)
        {
            IDictionary adapterMap = new Hashtable();
            IPicoContainer parent = container.Parent;
            if (parent != null)
            {
                IDictionary matchingComponentAdapters =
                    GetMatchingComponentAdapters(parent, adapter, keyType, valueType);

                foreach (DictionaryEntry entry in matchingComponentAdapters)
                {
                    adapterMap.Add(entry.Key, entry.Value);
                }
            }
            ICollection allAdapters = container.ComponentAdapters;

            foreach (IComponentAdapter componentAdapter in allAdapters)
            {
                adapterMap.Remove(componentAdapter.ComponentKey);
            }
            IList adapterList = container.GetComponentAdaptersOfType(valueType);

            foreach (IComponentAdapter componentAdapter in adapterList)
            {
                object key = componentAdapter.ComponentKey;
                if (adapter != null && key.Equals(adapter.ComponentKey))
                {
                    continue;
                }
                if (keyType.IsAssignableFrom(key.GetType()) && Evaluate(componentAdapter))
                {
                    adapterMap.Add(key, componentAdapter);
                }
            }
            return adapterMap;
        }

        private Type GetCollectionType(Type collectionType)
        {
            Type collectionClass = null;
            if (collectionType.IsArray)
            {
                collectionClass = typeof (Array);
            }
            else if (typeof (IDictionary).IsAssignableFrom(collectionType))
            {
                collectionClass = typeof (IDictionary);
            }
            else if (typeof (ICollection).IsAssignableFrom(collectionType))
            {
                collectionClass = typeof (ICollection);
            }
            return collectionClass;
        }

        private Type GetValueType(Type collectionType)
        {
            Type valueType = componentValueType;
            if (collectionType.IsArray)
            {
                valueType = collectionType.GetElementType();
            }
            return valueType;
        }

        private object[] GetArrayInstance(IPicoContainer container, Type expectedType, IDictionary adapterList)
        {
            object[] result = (object[]) Array.CreateInstance(expectedType.GetElementType(), adapterList.Count);
            int i = 0;

            foreach (DictionaryEntry entry in adapterList)
            {
                IComponentAdapter componentAdapter = (IComponentAdapter) entry.Value;
                result[i] = container.GetComponentInstance(componentAdapter.ComponentKey);
                i++;
            }
            return result;
        }

        private ICollection GetCollectionInstance(IPicoContainer container, Type expectedType, IDictionary adapterList)
        {
            Type collectionType = expectedType;
            if (collectionType.IsInterface)
            {
                // The order of tests are significant. The least generic types last.
                if (typeof (IList).IsAssignableFrom(collectionType))
                {
                    collectionType = typeof (ArrayList);
                }
//            } else if (BlockingQueue.class.isAssignableFrom(collectionType)) {
//                collectionType = ArrayBlockingQueue.class;
//            } else if (Queue.class.isAssignableFrom(collectionType)) {
//                collectionType = LinkedList.class;
                    /*} else if (SortedSet.class.isAssignableFrom(collectionType)) {
                collectionType = TreeSet.class;
            } else if (Set.class.isAssignableFrom(collectionType)) {
                collectionType = HashSet.class;
            }*/
                else if (typeof (ICollection).IsAssignableFrom(collectionType))
                {
                    collectionType = typeof (ArrayList);
                }
            }
            try
            {
                IList result = (IList) Activator.CreateInstance(collectionType);

                foreach (DictionaryEntry entry in adapterList)
                {
                    IComponentAdapter componentAdapter = (IComponentAdapter) entry.Value;
                    result.Add(container.GetComponentInstance(componentAdapter.ComponentKey));
                }

                return result;
            }
            catch (Exception e)
            {
                throw new PicoInitializationException(e);
            }
        }

        private IDictionary GetDictionaryInstance(IPicoContainer container, Type expectedType, IDictionary adapterList)
        {
            Type collectionType = expectedType;
            if (collectionType.IsInterface)
            {
                collectionType = typeof (Hashtable);
            }

            try
            {
                IDictionary result = (IDictionary) Activator.CreateInstance(collectionType);
                foreach (DictionaryEntry entry in adapterList)
                {
                    Object key = entry.Key;
                    result.Add(key, container.GetComponentInstance(key));
                }
                return result;
            }
            catch (Exception e)
            {
                throw new PicoInitializationException(e);
            }
        }
    }
}