
namespace PicoContainer
{
	/// <summary>
	/// Summary description for LifecycleManager.
	/// </summary>
	public interface ILifecycleManager
	{
		/// <summary>
		/// Invoke the "start" method on the container's components and child components.
		/// It is up to the implementor to define exactly what a component's "start" method is.
		/// </summary>
		/// <param name="node">The node to start the traversal.</param>
		void Start(IPicoContainer node);
		
		/// <summary>
		/// Invoke the "stop" method on the container's components and child components.
		/// It is up to the implementor to define exactly what a component's "stop" method is.
		/// </summary>
		/// <param name="node">The node to start the traversal.</param>
		void Stop(IPicoContainer node);

		/// <summary>
		/// Invoke the "dispose" method on the container's components and child components.
		/// It is up to the implementor to define exactly what a component's "dispose" method is.
		/// </summary>
		/// <param name="node">The node to start the traversal.</param>
		void Dispose(IPicoContainer node);
	}
}
