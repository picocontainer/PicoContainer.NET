using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using PicoContainer.Monitors;

namespace PicoContainer.Monitors
{
	[TestFixture]
	public class ConsoleComponentMonitorTestCase
	{
		private TextWriter writer = null;
		private ConsoleComponentMonitor componentMonitor = null;
		private ConstructorInfo constructor = null;
		private MethodInfo method;

		public override string ToString()
		{
			return "Blah";
		}

		[SetUp]
		protected void SetUp()
		{
			writer = new StringWriter();
			Type type = typeof (ConsoleComponentMonitorTestCase);
			constructor = type.GetConstructor(new Type[] {});
			method = type.GetMethod("ToString");

			componentMonitor = new ConsoleComponentMonitor(writer);
		}

		[Test]
		public void ShouldTraceInstantiating()
		{
			componentMonitor.Instantiating(constructor);
			Assert.AreEqual("PicoContainer: instantiating PicoContainer.Monitors.ConsoleComponentMonitorTestCase\r\n", writer.ToString());
		}

		[Test]
		public void ShouldTraceInstantiated()
		{
			componentMonitor.Instantiated(constructor, 1234, 543);
			Assert.AreEqual("PicoContainer: instantiated PicoContainer.Monitors.ConsoleComponentMonitorTestCase [543ms]\r\n", writer.ToString());
		}

		[Test]
		public void ShouldTraceInstantiationFailed()
		{
			componentMonitor.InstantiationFailed(constructor, new SystemException("doh"));
			Assert.AreEqual("PicoContainer: instantiation failed: PicoContainer.Monitors.ConsoleComponentMonitorTestCase, reason: 'doh'\r\n", writer.ToString());
		}

		[Test]
		public void ShouldTraceInvoking()
		{
			componentMonitor.Invoking(method, this);
			Assert.AreEqual("PicoContainer: invoking PicoContainer.Monitors.ConsoleComponentMonitorTestCase.ToString on Blah\r\n", writer.ToString());
		}

		[Test]
		public void ShouldTraceInvoked()
		{
			componentMonitor.Invoked(method, this, 543);
			Assert.AreEqual("PicoContainer: invoked PicoContainer.Monitors.ConsoleComponentMonitorTestCase.ToString on Blah [543ms]\r\n", writer.ToString());
		}

		[Test]
		public void ShouldTraceInvocatiationFailed()
		{
			componentMonitor.InvocationFailed(method, this, new SystemException("doh"));
			Assert.AreEqual("PicoContainer: invocation failed: PicoContainer.Monitors.ConsoleComponentMonitorTestCase.ToString on Blah, reason: 'doh'\r\n", writer.ToString());
		}
	}
}