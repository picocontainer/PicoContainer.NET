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

namespace PicoContainer.Defaults
{
    /// <summary>
    ///  A way to refer to objects that are stored in awkward places
    /// (for example HttpSession).
    /// <remarks>This is typically implemented by someone integrating Pico into
    /// an existing container.</remarks> 
    /// </summary>
    public interface IObjectReference
    {
        object Get();

        void Set(object item);
    }
}