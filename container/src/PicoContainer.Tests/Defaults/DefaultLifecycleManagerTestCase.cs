using System;
using System.Collections;
using System.Text;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;

namespace PicoContainer.Defaults
{
	/// <summary>
	/// Summary description for DefaultLifecycleManagerTestCase.
	/// </summary>
	[TestFixture]
	public class DefaultLifecycleManagerTestCase
	{
		private StringBuilder events;
		private object one;
		private object two;
		private object three;

		private Mock mock;
		private IPicoContainer pico;

		[SetUp]
		protected void SetUp()
		{
			events = new StringBuilder();
			one = new TestComponent("One", events);
			two = new TestComponent("Two", events);
			three = new TestComponent("Three", events);

			mock = new DynamicMock(typeof (IPicoContainer));
			pico = (IPicoContainer) mock.MockInstance;
		}

		[Test]
		public void StartingInOrder()
		{
			mock.ExpectAndReturn("GetComponentInstancesOfType",
			                     new ArrayList(new object[] {one, two, three}),
			                     new IsEqual(typeof (IStartable)));

			DefaultLifecycleManager dlm = new DefaultLifecycleManager();
			dlm.Start(pico);

			mock.Verify();

			Assert.AreEqual("<One<Two<Three", events.ToString());
		}

		[Test]
		public void StoppingInOrder()
		{
			mock.ExpectAndReturn("GetComponentInstancesOfType",
			                     new ArrayList(new object[] {one, two, three}),
			                     new IsEqual(typeof (IStartable)));

			DefaultLifecycleManager dlm = new DefaultLifecycleManager();
			dlm.Stop(pico);

			mock.Verify();

			Assert.AreEqual("Three>Two>One>", events.ToString());
		}

		[Test]
		public void DisposingInOrder()
		{
			mock.ExpectAndReturn("GetComponentInstancesOfType",
			                     new ArrayList(new object[] {one, two, three}),
			                     new IsEqual(typeof (IDisposable)));


			DefaultLifecycleManager dlm = new DefaultLifecycleManager();
			dlm.Dispose(pico);

			mock.Verify();
			Assert.AreEqual("!Three!Two!One", events.ToString());
		}

		class TestComponent : IStartable, IDisposable
		{
			private StringBuilder events;
			private string code;

			public TestComponent(string code, StringBuilder events)
			{
				this.code = code;
				this.events = events;
			}

			public void Start()
			{
				events.Append("<" + code);
			}

			public void Stop()
			{
				events.Append(code + ">");
			}

			public void Dispose()
			{
				events.Append("!" + code);
			}

		}
	}


}