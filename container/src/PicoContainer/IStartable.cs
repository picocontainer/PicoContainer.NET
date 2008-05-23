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

namespace PicoContainer
{
    /// <summary>
    /// An interface which is implemented by components that can be started and stopped. 
    /// </summary>
    /// <remarks>The <see cref="Start()"/>
    /// must be called at the begin of the component lifecycle. It can be called again only after a call to
    /// <see cref="Stop()"/>. The <see cref="Stop()"/> method must be called at the end of the component lifecycle,
    /// and can further be called after every <see cref="Start()"/>. If a component implements the <see cref="IDisposable"/>
    /// interface as well, <see cref="Stop()"/> should be called before <see cref="IDisposable.Dispose()"/>.</remarks>  
    public interface IStartable
    {
        /// <summary>
        /// Starts a component. 
        /// </summary>
        /// <remarks>Called initially at the begin of the lifecycle. It can be called again after a stop.</remarks>
        void Start();

        /// <summary>
        /// Stop this component. 
        /// </summary>
        /// <remarks>Called near the end the lifecycle.
        /// It can be called again after a further start. Implement <see cref="IDisposable"/> if you need a single call at the definite end of the lifecycle.
        /// </remarks>
        void Stop();
    }
}