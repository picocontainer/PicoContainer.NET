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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.TestModel;

namespace PicoContainer.Tck
{
	/// <summary>
	/// Summary description for AbstractPicoContainerTestCase.
	/// </summary>
	public abstract class AbstractPicoContainerTestCase
	{
		protected abstract IMutablePicoContainer CreatePicoContainer(IPicoContainer parent);
		protected abstract IMutablePicoContainer CreatePicoContainer(IPicoContainer parent, ILifecycleManager lifecycleManager);

		protected IMutablePicoContainer CreatePicoContainer()
		{
			return this.CreatePicoContainer(null);
		}

		protected IMutablePicoContainer CreatePicoContainerWithDependsOnTouchableOnly()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			pico.RegisterComponentImplementation(typeof (DependsOnTouchable));
			return pico;
		}

		protected IMutablePicoContainer CreatePicoContainerWithTouchableAndDependsOnTouchable()
		{
			IMutablePicoContainer pico = CreatePicoContainerWithDependsOnTouchableOnly();
			pico.RegisterComponentImplementation(typeof (ITouchable), typeof (SimpleTouchable));
			return pico;
		}

		[Test]
		public void NewContainerIsNotNull()
		{
			Assert.IsNotNull(CreatePicoContainerWithTouchableAndDependsOnTouchable());
		}

		[Test]
		public void RegisteredComponentsExistAndAreTheCorrectTypes()
		{
			IPicoContainer pico = CreatePicoContainerWithTouchableAndDependsOnTouchable();

			Assert.IsNotNull(pico.GetComponentAdapter(typeof (ITouchable)));
			Assert.IsNotNull(pico.GetComponentAdapter(typeof (DependsOnTouchable)));
			Assert.IsTrue(pico.GetComponentInstance(typeof (ITouchable)) is ITouchable);
			Assert.IsTrue(pico.GetComponentInstance(typeof (DependsOnTouchable)) is DependsOnTouchable);
			Assert.IsNull(pico.GetComponentAdapter(typeof (System.Collections.ICollection)));
		}

		[Test]
		public void RegistersSingleInstance()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			StringBuilder sb = new StringBuilder();
			pico.RegisterComponentInstance(sb);
			Assert.AreSame(sb, pico.GetComponentInstance(typeof (StringBuilder)));
		}

		[Test]
		public void ContainerIsSerializable()
		{
			ITouchable touchable = GetTouchableFromSerializedContainer();
			Assert.IsTrue(touchable.WasTouched);
		}

		private ITouchable GetTouchableFromSerializedContainer()
		{
			IMutablePicoContainer pico = CreatePicoContainerWithTouchableAndDependsOnTouchable();
			// Add a list too, using a constant parameter
			pico.RegisterComponentImplementation("list", typeof (ArrayList), new IParameter[] {new ConstantParameter(10)});

			using (Stream stream = new MemoryStream())
			{
				// Serialize it to memory
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, pico);

				// De-Serialize from memory
				stream.Seek(0, 0); // reset stream to begining
				pico = (IMutablePicoContainer) formatter.Deserialize(stream);
			}

			object dependsOnTouchable = pico.GetComponentInstance(typeof (DependsOnTouchable));
			Assert.IsNotNull(dependsOnTouchable);
			return (ITouchable) pico.GetComponentInstance(typeof (ITouchable));
		}

		[Test]
		public virtual void SerializedContainerCanRetrieveImplementation()
		{
			SimpleTouchable touchable = (SimpleTouchable)GetTouchableFromSerializedContainer();
			Assert.IsTrue(touchable.WasTouched);
		}

		[Test]
		public void GettingComponentWithMissingDependencyFails()
		{
			IPicoContainer picoContainer = CreatePicoContainerWithDependsOnTouchableOnly();
			try
			{
				picoContainer.GetComponentInstance(typeof (DependsOnTouchable));
				Assert.Fail("should need a Touchable");
			}
			catch (UnsatisfiableDependenciesException e)
			{
				Assert.AreSame(picoContainer.GetComponentAdapterOfType(typeof (DependsOnTouchable)).ComponentImplementation, e.UnsatisfiableComponentAdapter.ComponentImplementation);
				IList unsatisfiableDependencies = e.UnsatisfiableDependencies;
				Assert.AreEqual(1, unsatisfiableDependencies.Count);
				Assert.AreEqual(typeof (ITouchable), unsatisfiableDependencies[0]);
			}
		}

		[Test]
		public void DuplicateRegistration()
		{
			try
			{
				IMutablePicoContainer pico = CreatePicoContainer();
				pico.RegisterComponentImplementation(typeof (object));
				pico.RegisterComponentImplementation(typeof (object));
				Assert.Fail("Should have failed with duplicate registration");
			}
			catch (DuplicateComponentKeyRegistrationException e)
			{
				Assert.IsTrue(e.DuplicateKey == typeof (object));
			}
		}

		[Test]
		public void ExternallyInstantiatedObjectsCanBeRegistgeredAndLookUp()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			Hashtable map = new Hashtable();
			pico.RegisterComponentInstance(typeof (Hashtable), map);
			Assert.AreSame(map, pico.GetComponentInstance(typeof (Hashtable)));
		}

		[Test]
		public void AmbiguousResolution()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			pico.RegisterComponentImplementation("ping", typeof (string));
			pico.RegisterComponentInstance("pong", "pang");
			try
			{
				pico.GetComponentInstance(typeof (string));
			}
			catch (AmbiguousComponentResolutionException e)
			{
				Assert.IsTrue(e.Message.IndexOf("System.String") != -1);
			}
		}

		[Test]
		public void LookupWithUnregisteredKeyReturnsNull()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			Assert.IsNull(pico.GetComponentInstance(typeof (string)));
		}

		public class ListAdder
		{
			public ListAdder(IList list)
			{
				list.Add("something");
			}
		}

		[Test]
		public void UnsatisfiedComponentsExceptionGivesVerboseEnoughErrorMessage()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			pico.RegisterComponentImplementation(typeof (ComponentD));

			try
			{
				pico.GetComponentInstance(typeof (ComponentD));
			}
			catch (UnsatisfiableDependenciesException e)
			{
				IList unsatisfiableDependencies = e.UnsatisfiableDependencies;
				Assert.AreEqual(2, unsatisfiableDependencies.Count);
				Assert.IsTrue(unsatisfiableDependencies.Contains(typeof (ComponentE)));
				Assert.IsTrue(unsatisfiableDependencies.Contains(typeof (ComponentB)));

				Assert.IsTrue(e.Message.IndexOf(typeof (ComponentB).Name) != -1);
				Assert.IsTrue(e.Message.IndexOf(typeof (ComponentB).Name) != -1);
			}
		}

		[Test]
		public void CyclicDependencyThrowsCyclicDependencyException()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			pico.RegisterComponentImplementation(typeof (ComponentB));
			pico.RegisterComponentImplementation(typeof (ComponentD));
			pico.RegisterComponentImplementation(typeof (ComponentE));

			try
			{
				pico.GetComponentInstance(typeof (ComponentD));
				Assert.Fail();
			}
			catch (CyclicDependencyException e)
			{
				Type[] dDependencies = new Type[]{typeof(ComponentD), typeof(ComponentE), typeof(ComponentD)};
				Assert.AreEqual(dDependencies, e.Dependencies);
			}
			catch (Exception e)
			{
				Assert.Fail(e.Message);
			}
		}

		[Test]
		public void RemovalNonRegisteredIComponentAdapterWorksAndReturnsNull()
		{
			IMutablePicoContainer picoContainer = CreatePicoContainer();
			Assert.IsNull(picoContainer.UnregisterComponent("COMPONENT DOES NOT EXIST"));
		}

		[Test]
		public void IComponentAdapterRegistrationOrderIsMaintained()
		{
			ConstructorInjectionComponentAdapter c1 = new ConstructorInjectionComponentAdapter("1", typeof (object));
			ConstructorInjectionComponentAdapter c2 = new ConstructorInjectionComponentAdapter("2", typeof (MyString));

			IMutablePicoContainer picoContainer = CreatePicoContainer();
			picoContainer.RegisterComponent(c1);
			picoContainer.RegisterComponent(c2);
			ArrayList l = new ArrayList();
			l.Add(c1);
			l.Add(c2);

			object[] org = new object[] {c1, c2};
			for (int x = 0; x < 2; x++)
			{
				Assert.AreEqual(org[x], new ArrayList(picoContainer.ComponentAdapters).ToArray()[x]);
			}

			Assert.IsNotNull(picoContainer.ComponentInstances); // create all the instances at once
			Assert.IsFalse(picoContainer.ComponentInstances[0] is MyString);
			Assert.IsTrue(picoContainer.ComponentInstances[1] is MyString);

			IMutablePicoContainer reversedPicoContainer = CreatePicoContainer();
			reversedPicoContainer.RegisterComponent(c2);
			reversedPicoContainer.RegisterComponent(c1);

			l.Clear();
			l.Add(c2);
			l.Add(c1);
			org = new object[] {c2, c1};
			for (int x = 0; x < 2; x++)
			{
				Assert.AreEqual(org[x], new ArrayList(reversedPicoContainer.ComponentAdapters).ToArray()[x]);
			}

			Assert.IsNotNull(reversedPicoContainer.ComponentInstances); // create all the instances at once
			Assert.IsTrue(reversedPicoContainer.ComponentInstances[0] is MyString);
			Assert.IsFalse(reversedPicoContainer.ComponentInstances[1] is MyString);
		}

		public class MyString
		{
		}

		public class NeedsTouchable
		{
			public ITouchable touchable;

			public NeedsTouchable(ITouchable touchable)
			{
				this.touchable = touchable;
			}
		}

		public class NeedsWashable
		{
			public IWashable washable;

			public NeedsWashable(IWashable washable)
			{
				this.washable = washable;
			}
		}

		[Test]
		public void SameInstanceCanBeUsedAsDifferentType()
		{
			IMutablePicoContainer pico = CreatePicoContainer();
			pico.RegisterComponentImplementation("wt", typeof (WashableTouchable));
			pico.RegisterComponentImplementation("nw", typeof (NeedsWashable));
			pico.RegisterComponentImplementation("nt", typeof (NeedsTouchable));

			NeedsWashable nw = (NeedsWashable) pico.GetComponentInstance("nw");
			NeedsTouchable nt = (NeedsTouchable) pico.GetComponentInstance("nt");
			Assert.AreSame(nw.washable, nt.touchable);
		}

		[Test]
		public void RegisterComponentWithObjectBadType()
		{
			IMutablePicoContainer pico = CreatePicoContainer();

			try
			{
				pico.RegisterComponentInstance(typeof (IList), new Object());
				Assert.Fail("Shouldn't be able to register an Object.class as IList because it is not, " +
					"it does not implement it, Object.class does not implement much.");
			}
			catch (AssignabilityRegistrationException)
			{
			}
		}

		public class JMSService
		{
			public readonly String serverid;
			public readonly String path;

			public JMSService(String serverid, String path)
			{
				this.serverid = serverid;
				this.path = path;
			}
		}

		// http://jira.codehaus.org/secure/ViewIssue.jspa?key=PICO-52
		[Test]
		public void PicoIssue52()
		{
			IMutablePicoContainer pico = CreatePicoContainer();

			pico.RegisterComponentImplementation("foo", typeof (JMSService), new IParameter[]
				{
					new ConstantParameter("0"),
					new ConstantParameter("something"),
				});
			JMSService jms = (JMSService) pico.GetComponentInstance("foo");
			Assert.AreEqual("0", jms.serverid);
			Assert.AreEqual("something", jms.path);
		}

		public class ComponentA
		{
			public ComponentA(ComponentB b, ComponentC c)
			{
				Assert.IsNotNull(b);
				Assert.IsNotNull(c);
			}
		}

		public class ComponentB
		{
		}

		public class ComponentC
		{
		}

		public class ComponentD
		{
			public ComponentD(ComponentE e, ComponentB b)
			{
				Assert.IsNotNull(e);
				Assert.IsNotNull(b);
			}
		}

		public class ComponentE
		{
			public ComponentE(ComponentD d)
			{
				Assert.IsNotNull(d);
			}
		}

		public class ComponentF
		{
			public ComponentF(ComponentA a)
			{
				Assert.IsNotNull(a);
			}
		}

		[Test]
		public void MakingOfChildContainerPercolatesLifecycleManager()
		{
			TestLifecycleManager lifecycleManager = new TestLifecycleManager();
			IMutablePicoContainer parent = CreatePicoContainer(null, lifecycleManager);
			parent.RegisterComponentImplementation("one", typeof(TestLifecycleComponent));

			IMutablePicoContainer child = parent.MakeChildContainer();
			Assert.IsNotNull(child);
			child.RegisterComponentImplementation("two", typeof(TestLifecycleComponent));

			parent.Start();

			//TODO - The LifecycleManager reference in child containers is not used. Thus is is almost pointless
			// The reason is becuase DefaultPicoContainer's accept() method visits child containerson its own.
			// This may be file for visiting components in a tree for general cases, but for lifecycle, we
			// should hand to each LifecycleManager's start(..) at each appropriate node. See mail-list discussion.

			Assert.AreEqual(2, lifecycleManager.started.Count);
			//assertEquals(1, lifecycleManager.started.size());
		}

		[Test]
		public void StartStopAndDisposeNotCascadedtoRemovedChildren() 
		{
			IMutablePicoContainer parent = CreatePicoContainer(null);
			parent.RegisterComponentInstance(new StringBuilder());
			StringBuilder sb = (StringBuilder) ((IComponentAdapter)parent.GetComponentAdaptersOfType(typeof(StringBuilder))[0]).GetComponentInstance(parent);
			
			IMutablePicoContainer child = CreatePicoContainer(parent);
			Assert.IsTrue(parent.AddChildContainer(child));
			child.RegisterComponentImplementation(typeof(LifeCycleMonitoring));
			Assert.IsTrue(parent.RemoveChildContainer(child));
			parent.Start();
			Assert.IsTrue(sb.ToString().IndexOf("-started") == -1);
			parent.Stop();
			Assert.IsTrue(sb.ToString().IndexOf("-stopped") == -1);
			parent.Dispose();
			Assert.IsTrue(sb.ToString().IndexOf("-disposed") == -1);
		}

		public class LifeCycleMonitoring : IStartable, IDisposable
		{
			private StringBuilder sb;

			public LifeCycleMonitoring(StringBuilder sb)
			{
				this.sb = sb;
				sb.Append("-instantiated");
			}

			public void Start()
			{
				sb.Append("-started");
			}

			public void Stop()
			{
				sb.Append("-stopped");
			}

			public void Dispose()
			{
				sb.Append("-disposed");
			}
		}

		public class TestLifecycleComponent : IStartable
		{
			public bool started;

			public void Start()
			{
				started = true;
			}

			public void Stop()
			{
			}
		}

		public class TestLifecycleManager : DefaultLifecycleManager
		{
			public ArrayList started = new ArrayList();

			public override void Start(IPicoContainer node)
			{
				started.Add(node);
				base.Start(node);
			}
		}
	}
}