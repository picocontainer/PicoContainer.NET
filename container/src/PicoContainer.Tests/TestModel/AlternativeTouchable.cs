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

namespace PicoContainer.TestModel
{
    [Serializable]
    public class AlternativeTouchable : ITouchable
    {
        private bool wasTouched = false;

        #region ITouchable Members

        public bool WasTouched
        {
            get { return wasTouched; }
        }

        public virtual void Touch()
        {
            wasTouched = true;
        }

        #endregion
    }
}