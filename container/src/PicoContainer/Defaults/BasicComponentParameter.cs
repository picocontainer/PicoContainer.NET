using System;
using System.Collections;

namespace PicoContainer.Defaults
{
    [Serializable]
    public class BasicComponentParameter : IParameter
    {
        private readonly object componentKey;

        /**
         * Expect a parameter matching a component of a specific key.
         * 
         * @param componentKey the key of the desired component
         */
        public BasicComponentParameter(Object componentKey)
        {
            this.componentKey = componentKey;
        }

        /**
         * Expect any paramter of the appropriate type.
         */
        public BasicComponentParameter()
        {
        }

        /**
         * Check wether the given Parameter can be statisfied by the container.
         * 
         * @return <code>true</code> if the Parameter can be verified.
         * @see org.picocontainer.Parameter#isResolvable(org.picocontainer.PicoContainer,
         *           org.picocontainer.ComponentAdapter, java.lang.Class)
         */
        public virtual bool IsResolvable(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            return ResolveAdapter(container, adapter, expectedType) != null;
        }

        public virtual object ResolveInstance(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            IComponentAdapter componentAdapter = ResolveAdapter(container, adapter, expectedType);
            if (componentAdapter != null)
            {
                return container.GetComponentInstance(componentAdapter.ComponentKey);
            }
            return null;
        }

        public virtual void Verify(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            IComponentAdapter componentAdapter = ResolveAdapter(container, adapter, expectedType);
            if (componentAdapter == null)
            {
                throw new PicoIntrospectionException(expectedType.Name + " is not resolvable");
            }

            componentAdapter.Verify(container);
        }

        private IComponentAdapter ResolveAdapter(IPicoContainer container, IComponentAdapter adapter, Type expectedType)
        {
            IComponentAdapter result = GetTargetAdapter(container, expectedType, adapter);

            if (result != null && !expectedType.IsAssignableFrom(result.ComponentImplementation))
            {
                return null;
            }

            return result;
        }

        private IComponentAdapter GetTargetAdapter(IPicoContainer container, Type expectedType,
                                                   IComponentAdapter excludeAdapter)
        {
            if (componentKey != null)
            {
                // key tells us where to look so we follow
                return container.GetComponentAdapter(componentKey);
            }
            else if (excludeAdapter == null)
            {
                return container.GetComponentAdapterOfType(expectedType);
            }
            else
            {
                Object excludeKey = excludeAdapter.ComponentKey;
                IComponentAdapter byKey = container.GetComponentAdapter(expectedType);
                if (byKey != null)
                {
                    if (byKey.ComponentKey.Equals(excludeKey))
                    {
                        return null;
                    }
                    return byKey;
                }
                IList found = container.GetComponentAdaptersOfType(expectedType);
                IComponentAdapter exclude = null;

                foreach (IComponentAdapter work in found)
                {
                    if (work.ComponentKey.Equals(excludeKey))
                    {
                        exclude = work;
                    }
                }

                found.Remove(exclude);
                if (found.Count == 0)
                {
                    if (container.Parent != null)
                    {
                        return container.Parent.GetComponentAdapterOfType(expectedType);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (found.Count == 1)
                {
                    return (IComponentAdapter) found[0];
                }
                else
                {
                    Type[] foundTypes = new Type[found.Count];
                    for (int i = 0; i < foundTypes.Length; i++)
                    {
                        foundTypes[i] = ((IComponentAdapter) found[i]).ComponentImplementation;
                    }
                    throw new AmbiguousComponentResolutionException(expectedType, foundTypes);
                }
            }
        }
    }
}