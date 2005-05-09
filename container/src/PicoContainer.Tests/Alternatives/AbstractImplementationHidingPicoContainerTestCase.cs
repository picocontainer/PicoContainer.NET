using System;
using System.Collections;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Tck;

namespace PicoContainer.Alternatives
{
	[TestFixture]
	public abstract class AbstractImplementationHidingPicoContainerTestCase : AbstractPicoContainerTestCase
	{
		protected abstract IMutablePicoContainer CreateImplementationHidingPicoContainer();

		[Test]
		public void InstanceIsNotAutomaticallyHidden()
		{
			IMutablePicoContainer pc = CreateImplementationHidingPicoContainer();
			pc.RegisterComponentInstance(typeof (IDictionary), new Hashtable());

			IDictionary dictionary = (IDictionary) pc.GetComponentInstance(typeof (IDictionary));
			Assert.IsNotNull(dictionary);
			Assert.IsTrue(dictionary is Hashtable);
		}

		[Test]
		public void ImplementaionIsAutomaticallyHidden()
		{
			IMutablePicoContainer pc = CreateImplementationHidingPicoContainer();
			pc.RegisterComponentImplementation(typeof (IDictionary), typeof (Hashtable));
			IDictionary dictionary = (IDictionary) pc.GetComponentInstance(typeof (IDictionary));
			Assert.IsNotNull(dictionary);
			Assert.IsFalse(dictionary is Hashtable);
		}

		[Test]
		public void NonInterfaceImplementaionIsAutomaticallyHidden()
		{
			IMutablePicoContainer pc = CreateImplementationHidingPicoContainer();
			pc.RegisterComponentImplementation(typeof (Hashtable), typeof (Hashtable));
			IDictionary map = (IDictionary) pc.GetComponentInstance(typeof (Hashtable));
			Assert.IsNotNull(map);
			Assert.IsTrue(map is Hashtable);
		}

		[Test]
		public void NonInterfaceImplementaionWithParametersIsAutomaticallyHidden()
		{
			IMutablePicoContainer pc = CreateImplementationHidingPicoContainer();
			pc.RegisterComponentImplementation(typeof (Hashtable), typeof (Hashtable), new IParameter[0]);
			IDictionary map = (IDictionary) pc.GetComponentInstance(typeof (Hashtable));
			Assert.IsNotNull(map);
			Assert.IsTrue(map is Hashtable);
		}

		[Test]
		public void ImplementaionWithParametersIsAutomaticallyHidden()
		{
			IMutablePicoContainer pc = CreateImplementationHidingPicoContainer();
			pc.RegisterComponentImplementation(typeof (IDictionary), typeof (Hashtable), new IParameter[0]);
			IDictionary map = (IDictionary) pc.GetComponentInstance(typeof (IDictionary));
			Assert.IsNotNull(map);
			Assert.IsFalse(map is Hashtable);
		}

		[Test]
		public override void SerializedContainerCanRetrieveImplementation()
		{
			try
			{
				base.SerializedContainerCanRetrieveImplementation();
				Assert.Fail("The ImplementationHidingPicoContainer should not be able to retrieve the component impl");
			}
			catch (Exception ignore)
			{
				Console.WriteLine(ignore.StackTrace);
			}
		}

		/*

		public void testExceptionThrowingFromHiddenComponent()
		{
			IMutablePicoContainer pc = CreateImplementationHidingPicoContainer();
			pc.RegisterComponentImplementation(ActionListener.class,
			Burp.class)
			;
			try
			{
				ActionListener ac = (ActionListener) pc.getComponentInstance(ActionListener.class)
				;
				ac.actionPerformed(null);
				fail("Oh no.");
			}
			catch (RuntimeException e)
			{
				assertEquals("woohoo", e.getMessage());
			}
		}

		public static class Burp
		

		implements ActionListener
		{

		public void actionPerformed(ActionEvent e)
		{
			throw new RuntimeException("woohoo");
		}
	}*/

	}

}