using System;
using System.Reflection;

namespace PicoContainer.Defaults
{
	/// <summary>
	///
	/// </summary>
	public interface IComponentMonitor
	{
		void Instantiating(ConstructorInfo constructor);

		void Instantiated(ConstructorInfo constructor, long beforeTime, long duration);

		void InstantiationFailed(ConstructorInfo constructor, Exception e);

		void Invoking(MethodInfo method, Object instance);

		void Invoked(MethodInfo method, Object instance, long duration);

		void InvocationFailed(MethodInfo method, Object instance, Exception e);
	}
}
