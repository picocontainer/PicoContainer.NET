using System;

namespace PicoContainer.Defaults
{
    /// <summary>
    /// Abstract utility class to detect recursion cycles.  Derive from this class and implement 
    /// ThreadStaticCyclicDependencyGuard#Run The method will be called by 
    /// ThreadStaticCyclicDependencyGuard#Observe}. Selectan appropriate guard for your scope. 
    /// Any ObjectReference} can be used as long as it is initialized with FALSE
    /// </summary>
    [Serializable]
    public abstract class ThreadStaticCyclicDependencyGuard : ICyclicDependencyGuard
    {
        [ThreadStatic] private Boolean guardFlag = new Boolean();

        public ThreadStaticCyclicDependencyGuard()
        {
            guardFlag = false;
        }

        /**
		 * Derive from this class and implement this function with the functionality 
		 * to observe for a dependency cycle.
		 * 
		 * @return a value, if the functionality result in an expression, 
		 *      otherwise just return <code>null</code>
		 */

        #region ICyclicDependencyGuard Members

        public abstract object Run();

        /**
		 * Call the observing function. The provided guard will hold the {@link Boolean} value.
		 * If the guard is already <code>Boolean.TRUE</code> a {@link CyclicDependencyException} 
		 * will be  thrown.
		 * 
		 * @param stackFrame the current stack frame
		 * @return the result of the <code>run</code> method
		 */

        public Object Observe(Type stackFrame)
        {
            if (true.Equals(guardFlag))
            {
                throw new CyclicDependencyException(stackFrame);
            }
            Object result;

            try
            {
                guardFlag = true;
                result = Run();
            }
            catch (CyclicDependencyException e)
            {
                e.Push(stackFrame);
                throw e;
            }
            finally
            {
                guardFlag = false;
            }
            return result;
        }

        #endregion
    }
}