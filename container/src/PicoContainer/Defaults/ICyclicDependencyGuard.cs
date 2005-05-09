using System;

namespace PicoContainer.Defaults
{
	public interface ICyclicDependencyGuard
	{
		/// <summary>
		/// Derive from this class and implement this function with the functionality 
		/// to observe for a dependency cycle.
		/// </summary>
		/// <returns>if the functionality result in an expression, otherwise just return null</returns>
		object Run();
    
		/// <summary>
		/// Call the observing function. The provided guard will hold the bool value.
		/// If the guard is already <code>TRUE</code> a CyclicDependencyException
		/// will be  thrown.
		/// </summary>
		/// <param name="stackFrame">the current stack frame</param>
		/// <returns>the result of the <code>run</code> method</returns>
		object Observe(Type stackFrame);
	}
}
