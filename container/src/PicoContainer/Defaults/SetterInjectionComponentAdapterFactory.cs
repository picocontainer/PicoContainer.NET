using System;

namespace PicoContainer.Defaults
{
    public class SetterInjectionComponentAdapterFactory : IComponentAdapterFactory
    {
        private bool allowNonPublicClasses;

        public SetterInjectionComponentAdapterFactory(bool allowNonPublicClasses)
        {
            this.allowNonPublicClasses = allowNonPublicClasses;
        }

        public SetterInjectionComponentAdapterFactory() : this(false)
        {
        }

        #region IComponentAdapterFactory Members

        public IComponentAdapter CreateComponentAdapter(object componentKey, Type componentImplementation,
                                                        IParameter[] parameters)
        {
            return
                new SetterInjectionComponentAdapter(componentKey, componentImplementation, parameters,
                                                    allowNonPublicClasses);
        }

        #endregion
    }
}