using System;
using System.Collections;

namespace PicoContainer.Alternatives
{
    [Serializable]
    public abstract class AbstractDelegatingMutablePicoContainer : IMutablePicoContainer
    {
        private IMutablePicoContainer delegateContainer;

        public AbstractDelegatingMutablePicoContainer(IMutablePicoContainer delegateContainer)
        {
            this.delegateContainer = delegateContainer;
        }

        protected virtual IMutablePicoContainer DelegateContainer
        {
            get { return delegateContainer; }
        }

        #region IMutablePicoContainer Members

        public abstract IMutablePicoContainer MakeChildContainer();

        public virtual void AddOrderedComponentAdapter(IComponentAdapter componentAdapter)
        {
            delegateContainer.AddOrderedComponentAdapter(componentAdapter);
        }

        public virtual IComponentAdapter RegisterComponentImplementation(Object componentKey,
                                                                         Type componentImplementation)
        {
            return delegateContainer.RegisterComponentImplementation(componentKey, componentImplementation);
        }

        public virtual IComponentAdapter RegisterComponentImplementation(Object componentKey,
                                                                         Type componentImplementation,
                                                                         IParameter[] parameters)
        {
            return delegateContainer.RegisterComponentImplementation(componentKey, componentImplementation, parameters);
        }


        public virtual IComponentAdapter RegisterComponentImplementation(Type componentImplementation)
        {
            return delegateContainer.RegisterComponentImplementation(componentImplementation);
        }

        public virtual IComponentAdapter RegisterComponentInstance(Object componentInstance)
        {
            return delegateContainer.RegisterComponentInstance(componentInstance);
        }

        public virtual IComponentAdapter RegisterComponentInstance(Object componentKey, Object componentInstance)
        {
            return delegateContainer.RegisterComponentInstance(componentKey, componentInstance);
        }

        public virtual IComponentAdapter RegisterComponent(IComponentAdapter IComponentAdapter)
        {
            return delegateContainer.RegisterComponent(IComponentAdapter);
        }

        public virtual IComponentAdapter UnregisterComponent(Object componentKey)
        {
            return delegateContainer.UnregisterComponent(componentKey);
        }

        public virtual IComponentAdapter UnregisterComponentByInstance(Object componentInstance)
        {
            return delegateContainer.UnregisterComponentByInstance(componentInstance);
        }

        public virtual Object GetComponentInstance(Object componentKey)
        {
            return delegateContainer.GetComponentInstance(componentKey);
        }

        public virtual Object GetComponentInstanceOfType(Type componentType)
        {
            return delegateContainer.GetComponentInstanceOfType(componentType);
        }

        public virtual IList ComponentInstances
        {
            get { return delegateContainer.ComponentInstances; }
        }

        public virtual IPicoContainer Parent
        {
            get { return delegateContainer.Parent; }
        }

        public virtual IComponentAdapter GetComponentAdapter(Object componentKey)
        {
            return delegateContainer.GetComponentAdapter(componentKey);
        }

        public virtual IComponentAdapter GetComponentAdapterOfType(Type componentType)
        {
            return delegateContainer.GetComponentAdapterOfType(componentType);
        }

        public virtual IList ComponentAdapters
        {
            get { return delegateContainer.ComponentAdapters; }
        }

        public virtual IList GetComponentAdaptersOfType(Type componentType)
        {
            return delegateContainer.GetComponentAdaptersOfType(componentType);
        }

        public virtual void Verify()
        {
            delegateContainer.Verify();
        }

        public virtual void Start()
        {
            delegateContainer.Start();
        }

        public virtual void Stop()
        {
            delegateContainer.Stop();
        }

        public virtual void Dispose()
        {
            delegateContainer.Dispose();
        }

        public virtual bool AddChildContainer(IPicoContainer child)
        {
            return delegateContainer.AddChildContainer(child);
        }

        public virtual bool RemoveChildContainer(IPicoContainer child)
        {
            return delegateContainer.RemoveChildContainer(child);
        }

        public virtual IList GetComponentInstancesOfType(Type type)
        {
            return delegateContainer.GetComponentInstancesOfType(type);
        }

        #endregion

        /*public override bool Equals(Object obj)
		{
			return delegateContainer.Equals(obj);
		}

		public override int GetHashCode()
		{
			return delegateContainer.GetHashCode();
		}*/
    }
}