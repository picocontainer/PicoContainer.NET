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

namespace PicoContainer.Defaults
{
    [Serializable]
    public class DefaultPicoContainer : IMutablePicoContainer
    {
        private readonly IComponentAdapterFactory componentAdapterFactory;

        private readonly IList componentAdapters = new ArrayList();
        private readonly IDictionary componentKeyToAdapterMap = new Hashtable();
        private readonly ILifecycleManager lifecycleManager;
        private readonly IList orderedComponentAdapters = new ArrayList();

        private readonly Hashtable children = new Hashtable();
        private bool disposed = false;
        private IPicoContainer parent;
        private bool started = false;

        /// <summary>
        /// Creates a new container with a custom ComponentAdapterFactory and a parent container.
        /// Important note about caching: If you intend the components to be cached, you should pass
        /// in a factory that creates CachingComponentAdapter instances, such as for example
        /// other ComponentAdapterFactories.
        /// </summary>
        /// <param name="componentAdapterFactory">the factory to use for creation of ComponentAdapters.</param>
        /// <param name="parent">the parent container (used for component dependency lookups).</param>
        /// <param name="lifecycleManager">the lifecycle manager to manage start/stop/dispose calls on the container.</param>
        public DefaultPicoContainer(IComponentAdapterFactory componentAdapterFactory, IPicoContainer parent,
                                    ILifecycleManager lifecycleManager)
        {
            this.lifecycleManager = lifecycleManager;
            if (componentAdapterFactory == null)
            {
                throw new NullReferenceException("componentAdapterFactory");
            }
            this.componentAdapterFactory = componentAdapterFactory;
            this.parent = parent; // == null ? null : new ImmutablePicoContainer(parent);
        }

        public DefaultPicoContainer(IComponentAdapterFactory componentAdapterFactory,
                                    IPicoContainer parent)
            : this(componentAdapterFactory, parent, new DefaultLifecycleManager())
        {
        }

        public DefaultPicoContainer(IPicoContainer parent) : this(new DefaultComponentAdapterFactory(), parent)
        {
        }

        public DefaultPicoContainer(IComponentAdapterFactory componentAdapterFactory)
            : this(componentAdapterFactory, null)
        {
        }

        /// <summary>
        /// Creates a new container with a custom LifecycleManger and no parent container.*/
        /// </summary>
        /// <param name="lifecycleManager">the lifecycle manager to manage start/stop/dispose calls on the container.</param>
        public DefaultPicoContainer(ILifecycleManager lifecycleManager)
            : this(new DefaultComponentAdapterFactory(), null, lifecycleManager)
        {
        }

        public DefaultPicoContainer() : this(new DefaultComponentAdapterFactory(), null)
        {
        }

        #region IMutablePicoContainer Members

        public IList ComponentAdapters
        {
            get { return ArrayList.ReadOnly(componentAdapters); }
        }

        public IComponentAdapter GetComponentAdapter(object componentKey)
        {
            IComponentAdapter adapter = (IComponentAdapter) componentKeyToAdapterMap[componentKey];
            if (adapter == null && parent != null)
            {
                adapter = parent.GetComponentAdapter(componentKey);
            }

            return adapter;
        }

        public IComponentAdapter GetComponentAdapterOfType(Type componentType)
        {
            IComponentAdapter adapterByKey = GetComponentAdapter(componentType);
            if (adapterByKey != null)
            {
                return adapterByKey;
            }

            IList found = GetComponentAdaptersOfType(componentType);

            if (found.Count == 1)
            {
                return (IComponentAdapter) found[0];
            }

            if (found.Count == 0)
            {
                if (parent != null)
                {
                    return parent.GetComponentAdapterOfType(componentType);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Type[] foundClasses = new Type[found.Count];
                for (int i = 0; i < foundClasses.Length; i++)
                {
                    IComponentAdapter componentAdapter = (IComponentAdapter) found[i];
                    foundClasses[i] = componentAdapter.ComponentImplementation;
                }

                throw new AmbiguousComponentResolutionException(componentType, foundClasses);
            }
        }

        public IList GetComponentAdaptersOfType(Type componentType)
        {
            IList found = new ArrayList();

            foreach (IComponentAdapter componentAdapter in ComponentAdapters)
            {
                if (componentType.IsAssignableFrom(componentAdapter.ComponentImplementation))
                {
                    found.Add(componentAdapter);
                }
            }
            return found;
        }

        public IComponentAdapter RegisterComponent(IComponentAdapter componentAdapter)
        {
            object componentKey = componentAdapter.ComponentKey;
            if (componentKeyToAdapterMap.Contains(componentKey))
            {
                throw new DuplicateComponentKeyRegistrationException(componentKey);
            }
            componentAdapter.Container = this;
            componentAdapters.Add(componentAdapter);
            componentKeyToAdapterMap.Add(componentKey, componentAdapter);

            return componentAdapter;
        }

        public IComponentAdapter UnregisterComponent(object componentKey)
        {
            IComponentAdapter adapter = (IComponentAdapter) componentKeyToAdapterMap[componentKey];
            if (adapter != null)
            {
                componentKeyToAdapterMap.Remove(componentKey);
                componentAdapters.Remove(adapter);
                orderedComponentAdapters.Remove(adapter);
            }

            return adapter;
        }

        public IComponentAdapter RegisterComponentInstance(object component)
        {
            return RegisterComponentInstance(component.GetType(), component);
        }

        public IComponentAdapter RegisterComponentInstance(object componentKey, object componentInstance)
        {
            if (componentInstance == this)
            {
                throw new PicoRegistrationException(
                    "Can not register a container to itself. The container is already implicitly registered.");
            }

            IComponentAdapter componentAdapter = new InstanceComponentAdapter(componentKey, componentInstance);

            RegisterComponent(componentAdapter);

            return componentAdapter;
        }


        public IComponentAdapter RegisterComponentImplementation(Type componentImplementation)
        {
            return RegisterComponentImplementation(componentImplementation, componentImplementation);
        }

        public IComponentAdapter RegisterComponentImplementation(object componentKey, Type componentImplementation)
        {
            return RegisterComponentImplementation(componentKey, componentImplementation, null);
        }

        public IComponentAdapter RegisterComponentImplementation(object componentKey, Type componentImplementation,
                                                                 IParameter[] parameters)
        {
            IComponentAdapter componentAdapter =
                componentAdapterFactory.CreateComponentAdapter(componentKey, componentImplementation, parameters);

            RegisterComponent(componentAdapter);

            return componentAdapter;
        }


        public void AddOrderedComponentAdapter(IComponentAdapter componentAdapter)
        {
            if (!orderedComponentAdapters.Contains(componentAdapter))
            {
                orderedComponentAdapters.Add(componentAdapter);
            }
        }

        public IList ComponentInstances
        {
            get { return GetComponentInstancesOfType(typeof (object)); }
        }

        public IList GetComponentInstancesOfType(Type componentType)
        {
            if (componentType == null)
            {
                return new ArrayList();
            }

            Hashtable adapterToInstanceMap = new Hashtable();
            foreach (IComponentAdapter componentAdapter in componentAdapters)
            {
                if (componentType.IsAssignableFrom(componentAdapter.ComponentImplementation))
                {
                    Object componentInstance = GetInstance(componentAdapter);
                    adapterToInstanceMap.Add(componentAdapter, componentInstance);

                    // This is to ensure all are added. (Indirect dependencies will be added
                    // from InstantiatingComponentAdapter).
                    AddOrderedComponentAdapter(componentAdapter);
                }
            }

            IList result = new ArrayList();
            foreach (IComponentAdapter componentAdapter in orderedComponentAdapters)
            {
                Object componentInstance = adapterToInstanceMap[componentAdapter];
                if (componentInstance != null)
                {
                    // may be null in the case of the "implicit" adapter
                    // representing "this".
                    result.Add(componentInstance);
                }
            }

            return result;
        }

        public object GetComponentInstance(object componentKey)
        {
            IComponentAdapter componentAdapter = GetComponentAdapter(componentKey);
            if (componentAdapter != null)
            {
                return GetInstance(componentAdapter);
            }
            else
            {
                return null;
            }
        }

        public object GetComponentInstanceOfType(Type componentType)
        {
            IComponentAdapter componentAdapter = GetComponentAdapterOfType(componentType);
            return componentAdapter == null ? null : FindInstance(componentAdapter);
        }

        public IComponentAdapter UnregisterComponentByInstance(object componentInstance)
        {
            foreach (IComponentAdapter componentAdapter in ComponentAdapters)
            {
                if (componentAdapter.GetComponentInstance(this).Equals(componentInstance))
                {
                    return UnregisterComponent(componentAdapter.ComponentKey);
                }
            }
            return null;
        }

        public IMutablePicoContainer MakeChildContainer()
        {
            DefaultPicoContainer pc = new DefaultPicoContainer(componentAdapterFactory, this, lifecycleManager);
            AddChildContainer(pc);
            return pc;
        }

        public virtual bool AddChildContainer(IPicoContainer child)
        {
            if (children.Contains(child))
            {
                return false;
            }
            children.Add(child, child);
            return true;
        }

        public virtual bool RemoveChildContainer(IPicoContainer child)
        {
            if (children.Contains(child))
            {
                children.Remove(child);
                return true;
            }

            return false;
        }

        public IPicoContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public void Verify()
        {
            IList nestedVerificationExceptions = new ArrayList();
            foreach (IComponentAdapter componentAdapter in ComponentAdapters)
            {
                try
                {
                    componentAdapter.Verify(this);
                }
                catch (UnsatisfiableDependenciesException e)
                {
                    nestedVerificationExceptions.Add(e);
                }
            }

            if (nestedVerificationExceptions.Count > 0)
            {
                throw new PicoVerificationException(nestedVerificationExceptions);
            }
        }

        public void Start()
        {
            if (started) throw new ApplicationException("Already started");
            if (disposed) throw new ApplicationException("Already disposed");

            lifecycleManager.Start(this);
            foreach (DictionaryEntry child in children)
            {
                IPicoContainer pico = (IPicoContainer) child.Value;
                pico.Start();
            }

            started = true;
        }

        public void Stop()
        {
            if (disposed) throw new ApplicationException("Already disposed");
            if (!started) throw new ApplicationException("Not started");

            foreach (DictionaryEntry child in children)
            {
                IPicoContainer pico = (IPicoContainer) child.Value;
                pico.Stop();
            }
            lifecycleManager.Stop(this);
            started = false;
        }

        public void Dispose()
        {
            if (disposed) throw new SystemException("Already disposed");
            foreach (DictionaryEntry child in children)
            {
                IPicoContainer pico = (IPicoContainer) child.Value;
                pico.Dispose();
            }
            lifecycleManager.Dispose(this);
            disposed = true;
        }

        #endregion

        private object GetInstance(IComponentAdapter componentAdapter)
        {
            // check wether this is our adapter
            // we need to check this to ensure up-down dependencies cannot be followed
            bool isLocal = componentAdapters.Contains(componentAdapter);

            if (isLocal)
            {
                Object instance = componentAdapter.GetComponentInstance(this);
                AddOrderedComponentAdapter(componentAdapter);

                return instance;
            }
            else if (parent != null)
            {
                return parent.GetComponentInstance(componentAdapter.ComponentKey);
            }

            // TODO: decide .. exception or null?
            // exceptrion: mx: +1, joehni +1
            return null;
        }

        private object FindInstance(IComponentAdapter componentAdapter)
        {
            // check wether this is our adapter
            // we need to check this to ensure up-down dependencies cannot be followed
            bool isLocal = componentAdapters.Contains(componentAdapter);

            if (isLocal)
            {
                Object instance = componentAdapter.GetComponentInstance(this);

                AddOrderedComponentAdapter(componentAdapter);

                return instance;
            }
            else if (parent != null)
            {
                return parent.GetComponentInstance(componentAdapter.ComponentKey);
            }

            // TODO: decide .. exception or null?
            // exceptrion: mx: +1, joehni +1
            return null;
        }

        public static IList OrderComponentAdaptersWithContainerAdaptersLast(IList ComponentAdapters)
        {
            ArrayList result = new ArrayList();
            ArrayList containers = new ArrayList();
            foreach (IComponentAdapter adapter in ComponentAdapters)
            {
                if (typeof (IPicoContainer).IsAssignableFrom(adapter.ComponentImplementation))
                {
                    containers.Add(adapter);
                }
                else
                {
                    result.Add(adapter);
                }
            }
            result.AddRange(containers);

            return result;
        }
    }
}