using System;
using System.IO;
using System.Reflection;
using PicoContainer.Defaults;

namespace PicoContainer.Monitors
{
	/// <summary>
	/// Implementation of ComponentMonitor which logs to the Console.
	/// </summary>
	[Serializable]
	public class ConsoleComponentMonitor : IComponentMonitor
	{
		private TextWriter writer;

		public ConsoleComponentMonitor(TextWriter writer)
		{
			this.writer = writer;
		}

		public void Instantiating(ConstructorInfo constructor)
		{
			writer.WriteLine("PicoContainer: instantiating {0}", constructor.DeclaringType.FullName);
		}

		public void Instantiated(ConstructorInfo constructor, long beforeTime, long duration)
		{
			writer.WriteLine("PicoContainer: instantiated {0} [{1}ms]", constructor.DeclaringType.FullName, duration);
		}

		public void InstantiationFailed(ConstructorInfo constructor, Exception e)
		{
			writer.WriteLine("PicoContainer: instantiation failed: {0}, reason: '{1}'",
			                 constructor.DeclaringType.FullName,
			                 e.Message);
		}

		public void Invoking(MethodInfo method, object instance)
		{
			writer.WriteLine("PicoContainer: invoking {0}.{1} on {2}",
			                 method.DeclaringType.FullName,
			                 method.Name,
			                 instance);
		}

		public void Invoked(MethodInfo method, Object instance, long duration)
		{
			writer.WriteLine("PicoContainer: invoked {0}.{1} on {2} [{3}ms]",
			                 method.DeclaringType.FullName,
			                 method.Name,
			                 instance,
			                 duration);
		}

		public void InvocationFailed(MethodInfo method, Object instance, Exception e)
		{
			writer.WriteLine("PicoContainer: invocation failed: {0}.{1} on {2}, reason: '{3}'",
			                 method.DeclaringType.FullName,
			                 method.Name,
			                 instance,
			                 e.Message);
		}

	}
}