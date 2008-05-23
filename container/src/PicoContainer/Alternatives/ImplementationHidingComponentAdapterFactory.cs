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
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    /// <summary>
    /// Summary description for ImplementationHidingComponentAdapterFactory.
    /// </summary>
    [Serializable]
    public class ImplementationHidingComponentAdapterFactory : DecoratingComponentAdapterFactory
    {
        private readonly bool strict;

        public ImplementationHidingComponentAdapterFactory() : this(new DefaultComponentAdapterFactory(), true)
        {
        }

        public ImplementationHidingComponentAdapterFactory(IComponentAdapterFactory theDelegate)
            : this(theDelegate, true)
        {
        }

        public ImplementationHidingComponentAdapterFactory(IComponentAdapterFactory theDelegate, bool strict)
            : base(theDelegate)
        {
            this.strict = strict;
        }

        public override IComponentAdapter CreateComponentAdapter(object componentKey,
                                                                 Type componentImplementation,
                                                                 IParameter[] parameters)
        {
            return
                new ImplementationHidingComponentAdapter(
                    base.CreateComponentAdapter(componentKey, componentImplementation, parameters), strict);
        }
    }
}