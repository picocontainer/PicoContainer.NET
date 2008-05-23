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
using System.Collections;
using PicoContainer.Utils;

namespace PicoContainer.Defaults
{
    [Serializable]
    public class CyclicDependencyException : PicoInitializationException
    {
        private readonly Stack stack;

        public CyclicDependencyException(Type element) : base((Exception) null)
        {
            stack = new Stack();
            Push(element);
        }

        public object[] Dependencies
        {
            get { return stack.ToArray(); }
        }

        public override string Message
        {
            get { return "Cyclic dependency: " + StringUtils.ArrayToString(stack.ToArray()); }
        }

        public void Push(Type element)
        {
            stack.Push(element);
        }
    }
}