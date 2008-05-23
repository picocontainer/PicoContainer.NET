using System;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    /// <summary>
    /// Summary description for ImplementationHidingCachingPicoContainer.
    /// </summary>
    [Serializable]
    public class ImplementationHidingCachingPicoContainer : AbstractDelegatingMutablePicoContainer
    {
        private CachingComponentAdapterFactory caf;
        private ILifecycleManager lifecycleManager;

        /// <summary>
        /// Creates a new container with a parent container.
        /// </summary>
        /// <param name="caf"></param>
        /// <param name="parent"></param>
        public ImplementationHidingCachingPicoContainer(IComponentAdapterFactory caf, IPicoContainer parent)
            : this(parent, new CachingComponentAdapterFactory(caf), new DefaultLifecycleManager())
        {
        }

        public ImplementationHidingCachingPicoContainer(IComponentAdapterFactory caf, IPicoContainer parent,
                                                        ILifecycleManager lifecyleManager)
            : this(parent, new CachingComponentAdapterFactory(caf), lifecyleManager)
        {
        }

        private ImplementationHidingCachingPicoContainer(IPicoContainer parent, CachingComponentAdapterFactory caf,
                                                         ILifecycleManager lifecycleManager)
            : base(new ImplementationHidingPicoContainer(caf, parent, lifecycleManager))
        {
            this.caf = caf;
            this.lifecycleManager = lifecycleManager;
        }

        /// <summary>
        /// Creates a new container with a parent container.
        /// </summary>
        /// <param name="parent"></param>
        public ImplementationHidingCachingPicoContainer(IPicoContainer parent)
            : this(new DefaultComponentAdapterFactory(), parent)
        {
        }

        /// <summary>
        /// Creates a new container with no parent container.
        /// </summary>
        public ImplementationHidingCachingPicoContainer() : this(null)
        {
        }

        public override IComponentAdapter RegisterComponentImplementation(Object componentKey,
                                                                          Type componentImplementation)
        {
            if (componentKey is Type)
            {
                Type clazz = (Type) componentKey;
                if (clazz.IsInterface)
                {
                    IComponentAdapter caDelegate =
                        caf.CreateComponentAdapter(componentKey, componentImplementation, null);
                    return
                        DelegateContainer.RegisterComponent(
                            new CachingComponentAdapter(new ImplementationHidingComponentAdapter(caDelegate, true)));
                }
            }
            return DelegateContainer.RegisterComponentImplementation(componentKey, componentImplementation);
        }

        public override IComponentAdapter RegisterComponentImplementation(Object componentKey,
                                                                          Type componentImplementation,
                                                                          IParameter[] parameters)
        {
            if (componentKey is Type)
            {
                Type clazz = (Type) componentKey;
                if (clazz.IsInterface)
                {
                    IComponentAdapter caDelegate =
                        caf.CreateComponentAdapter(componentKey, componentImplementation, parameters);
                    ImplementationHidingComponentAdapter ihDelegate =
                        new ImplementationHidingComponentAdapter(caDelegate, true);
                    return DelegateContainer.RegisterComponent(new CachingComponentAdapter(ihDelegate));
                }
            }
            return DelegateContainer.RegisterComponentImplementation(componentKey, componentImplementation, parameters);
        }


        public override IMutablePicoContainer MakeChildContainer()
        {
            ImplementationHidingCachingPicoContainer pc =
                new ImplementationHidingCachingPicoContainer(this, caf, lifecycleManager);
            DelegateContainer.AddChildContainer(pc);
            return pc;
        }
    }
}