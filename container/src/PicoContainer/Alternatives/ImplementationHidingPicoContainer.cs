using System;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    [Serializable]
    public class ImplementationHidingPicoContainer : AbstractDelegatingMutablePicoContainer
    {
        private readonly IComponentAdapterFactory caf;
        private readonly ILifecycleManager lifecycleManager;

        /// <summary>
        /// Creates a new container with a parent container.
        /// </summary>
        /// <param name="caf"></param>
        /// <param name="parent"></param>
        /// <param name="lifecycleManager"></param>
        public ImplementationHidingPicoContainer(IComponentAdapterFactory caf, IPicoContainer parent,
                                                 ILifecycleManager lifecycleManager)
            : base(new DefaultPicoContainer(caf, parent, lifecycleManager))
        {
            this.caf = caf;
            this.lifecycleManager = lifecycleManager;
        }

        public ImplementationHidingPicoContainer(IComponentAdapterFactory caf, IPicoContainer parent)
            : this(caf, parent, new DefaultLifecycleManager())
        {
        }

        /// <summary>
        /// Creates a new container with a parent container.
        /// </summary>
        /// <param name="parent"></param>
        public ImplementationHidingPicoContainer(IPicoContainer parent)
            : this(new DefaultComponentAdapterFactory(), parent)
        {
        }

        /// <summary>
        /// Creates a new container with no parent container.
        /// </summary>
        public ImplementationHidingPicoContainer() : this(null)
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
                        DelegateContainer.RegisterComponent(new ImplementationHidingComponentAdapter(caDelegate, true));
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
                    return DelegateContainer.RegisterComponent(ihDelegate);
                }
            }
            return DelegateContainer.RegisterComponentImplementation(componentKey, componentImplementation, parameters);
        }

        public override IMutablePicoContainer MakeChildContainer()
        {
            ImplementationHidingPicoContainer pc = new ImplementationHidingPicoContainer(caf, this, lifecycleManager);
            DelegateContainer.AddChildContainer(pc);
            return pc;
        }
    }
}