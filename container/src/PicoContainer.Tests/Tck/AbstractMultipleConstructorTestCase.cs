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
using PicoContainer;
using PicoContainer.Defaults;
using NUnit.Framework;

namespace PicoContainer.Tck
{
	/// <summary>
	/// Summary description for AbstractMultipleConstructorTestCase.
	/// </summary>
	public abstract class AbstractMultipleConstructorTestCase
	{
		protected abstract IMutablePicoContainer createPicoContainer();

		public class Multi
		{
			public String message;

			public Multi(One one, Two two, Three three)
			{
				message = "one two three";
			}

			public Multi(One one, Two two)
			{
				message = "one two";
			}

			public Multi(Two two, One one)
			{
				message = "two one";
			}

			public Multi(Two two, Three three)
			{
				message = "two three";
			}

			public Multi(Three three, One one)
			{
				message = "three one";
			}
		}

		public class One
		{
		}

		public class Two
		{
		}

		public class Three
		{
		}

		[Test]
		public void StringWorks()
		{
			// Difference in java and .net
		}


		[Test]
		public void MultiWithOnlySmallSatisfiedDependencyWorks()
		{
			IMutablePicoContainer pico = createPicoContainer();
			pico.RegisterComponentImplementation(typeof (Multi));
			pico.RegisterComponentImplementation(typeof (One));
			pico.RegisterComponentImplementation(typeof (Three));

			Multi multi = (Multi) pico.GetComponentInstance(typeof (Multi));
			Assert.AreEqual("three one", multi.message);
		}

		[Test]
		public void MultiWithBothSatisfiedDependencyWorks()
		{
			IMutablePicoContainer pico = createPicoContainer();
			pico.RegisterComponentImplementation(typeof (Multi));
			pico.RegisterComponentImplementation(typeof (One));
			pico.RegisterComponentImplementation(typeof (Two));
			pico.RegisterComponentImplementation(typeof (Three));

			Multi multi = (Multi) pico.GetComponentInstance(typeof (Multi));
			Assert.AreEqual("one two three", multi.message);
		}

		[Test]
		public void MultiWithTwoEquallyBigSatisfiedDependenciesFails()
		{
			IMutablePicoContainer pico = createPicoContainer();
			pico.RegisterComponentImplementation(typeof (Multi));
			pico.RegisterComponentImplementation(typeof (One));
			pico.RegisterComponentImplementation(typeof (Two));

			try
			{
				Multi multi = (Multi) pico.GetComponentInstance(typeof (Multi));
				Assert.Fail();
			}
			catch (TooManySatisfiableConstructorsException e)
			{
				Assert.IsTrue(e.Message.IndexOf("Three") == -1);
				Assert.AreEqual(2, e.Constructors.Count);
				Assert.AreEqual(typeof (Multi), e.ForImplementationClass);
			}
		}
	}
}