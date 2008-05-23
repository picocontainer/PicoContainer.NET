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
    /// Summary description for SynchronizedComponentAdapterFactory.
    /// </summary>
    [Serializable]
    public class SynchronizedComponentAdapterFactory : DecoratingComponentAdapterFactory
    {
        public SynchronizedComponentAdapterFactory(IComponentAdapterFactory theDelegate) : base(theDelegate)
        {
        }

        public override IComponentAdapter CreateComponentAdapter(object componentKey, Type componentImplementation,
                                                                 IParameter[] parameters)
        {
            return
                new SynchronizedComponentAdapter(
                    base.CreateComponentAdapter(componentKey, componentImplementation, parameters));
        }
    }
}