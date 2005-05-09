using System;
using System.Collections;
using System.Reflection;
using PicoContainer;

namespace PicoContainer.Defaults
{
	/// <summary>
	/// Summary description for DefaultLifecycleManager.
	/// </summary
	[Serializable]
	public class DefaultLifecycleManager : ILifecycleManager
	{
		protected IComponentMonitor componentMonitor;

		protected static MethodInfo startMethod;
		protected static MethodInfo stopMethod;
		protected static MethodInfo disposeMethod;

		private static object[] emptyArray = new object[0];

		static DefaultLifecycleManager()
		{
			startMethod = typeof (IStartable).GetMethod("Start");
			stopMethod = typeof (IStartable).GetMethod("Stop");
			disposeMethod = typeof (IDisposable).GetMethod("Dispose");
		}

		public DefaultLifecycleManager()
		{
			this.componentMonitor = new NullComponentMonitor();
		}

		public DefaultLifecycleManager(IComponentMonitor componentMonitor)
		{
			this.componentMonitor = componentMonitor;
		}

		public virtual void Start(IPicoContainer node)
		{
			IList startables = node.GetComponentInstancesOfType(typeof (IStartable));

			foreach (object startable in startables)
			{
				DoMethod(startMethod, startable);
			}
		}

		public virtual void Stop(IPicoContainer node)
		{
			IList startables = node.GetComponentInstancesOfType(typeof (IStartable));

			for (int i = startables.Count - 1; 0 <= i; i--)
			{
				DoMethod(stopMethod, startables[i]);
			}
		}

		public virtual void Dispose(IPicoContainer node)
		{
			IList disposables = node.GetComponentInstancesOfType(typeof (IDisposable));
			for (int i = disposables.Count - 1; 0 <= i; i--)
			{
				DoMethod(disposeMethod, disposables[i]);
			}
		}

		protected virtual void DoMethod(MethodInfo method, Object instance)
		{
			componentMonitor.Invoking(method, instance);
			try
			{
				long beginTime = DateTime.Now.Millisecond;
				method.Invoke(instance, emptyArray);
				componentMonitor.Invoked(method, instance, DateTime.Now.Millisecond - beginTime);
			}
			catch (Exception e)
			{
				InvocationFailed(method, instance, e);
			}
		}

		protected virtual void InvocationFailed(MethodInfo method, Object instance, Exception e)
		{
			componentMonitor.InvocationFailed(method, instance, e);
			string message = string.Format("Method '{0}' failed on instance '{1}' for reason '{2}'",
			                               method.Name,
			                               instance,
			                               e.Message);

			throw new PicoInitializationException(message, e);
		}
	}
}