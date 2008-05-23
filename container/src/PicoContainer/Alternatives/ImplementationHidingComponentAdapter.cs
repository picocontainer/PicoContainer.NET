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
using Castle.DynamicProxy;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    [Serializable]
    public class ImplementationHidingComponentAdapter : DecoratingComponentAdapter
    {
        private bool strict = false;

        public ImplementationHidingComponentAdapter(IComponentAdapter theDelegate, bool strict) : base(theDelegate)
        {
            this.strict = strict;
        }

        public ImplementationHidingComponentAdapter(IComponentAdapter theDelegate) : base(theDelegate)
        {
        }

        public override Object GetComponentInstance(IPicoContainer container)
        {
            Object componentKey = Delegate.ComponentKey;
            Type[] types = null;
            if (componentKey is Type && ((Type) Delegate.ComponentKey).IsInterface)
            {
                types = new Type[] {(Type) Delegate.ComponentKey};
            }
            else if (componentKey is Type[])
            {
                types = (Type[]) componentKey;
            }
            else
            {
                if (strict)
                {
                    throw new PicoIntrospectionException("In strict mode, "
                                                         + GetType().Name
                                                         +
                                                         " only allows components registered with interface keys (System.Type or System.Type[])");
                }
                return Delegate.GetComponentInstance(container);
            }

            return CreateProxy(types, container);
        }

        private Object CreateProxy(Type[] interfaces, IPicoContainer container)
        {
            try
            {
                ProxyGenerator proxyGenerator = new ProxyGenerator();
                object componentInstance = Delegate.GetComponentInstance(container);
                return proxyGenerator.CreateProxy(interfaces, new PicoInterceptor(), componentInstance);
            }
            catch (ArgumentException e)
            {
                throw new PicoIntrospectionException("Error creating a dynamic proxy for implementation hiding", e);
            }
        }

        #region Nested type: PicoInterceptor

        internal class PicoInterceptor : IInterceptor
        {
            #region IInterceptor Members

            public object Intercept(IInvocation invocation, params object[] args)
            {
                return invocation.Proceed(args);
            }

            #endregion
        }

        #endregion
    }
}