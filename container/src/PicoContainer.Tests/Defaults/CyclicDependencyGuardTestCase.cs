using System.Threading;
using NUnit.Framework;
using PicoContainer.Defaults;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class CyclicDependencyGuardTestCase
	{
		[Test]
		public void CyclicDependencyWithThreadSafeGuard() 
		{
			ThreadLocalRunner[] runner = new ThreadLocalRunner[3];

			for(int i = 0; i < runner.Length; ++i) 
			{
				runner[i] = new ThreadLocalRunner();
			}
        
			initTest(runner);

			for(int i = 0; i < runner.Length; ++i) 
			{
				Assert.IsNull((runner[i]).exception);
			}
		}

		class ThreadLocalRunner
		{
			public CyclicDependencyException exception;
			public ThreadStart threadStart;
			private Blocker blocker;
			private ICyclicDependencyGuard guard;

			public ThreadLocalRunner()
			{
				this.blocker = new Blocker();
				this.guard = new SampleThreadLocalCyclicDependencyGuard(blocker);

				this.threadStart = new ThreadStart(Run);
			}
			
			// passed to ThreadStart
			public void Run() 
			{
				try 
				{
					guard.Observe(typeof(ThreadLocalRunner));
				} 
				catch (CyclicDependencyException e) 
				{
					exception = e;
				}
			}
		}


		private void initTest(ThreadLocalRunner[] runners)
		{
			Thread[] threads = new Thread[runners.Length];

			// build threads
			for (int i = 0; i < threads.Length; ++i)
			{
				threads[i] = new Thread(runners[i].threadStart);
			}

			// kick-off each thread
			foreach (Thread thread in threads)
			{
				thread.Start();
				Thread.Sleep(200);
			}

			// 
			foreach (Thread thread in threads)
			{	
				lock (thread)
				{
					Monitor.PulseAll(thread);
				}
			}

			foreach (Thread thread in threads)
			{
				thread.Join();
			}
		}

		protected class Blocker
		{
			public void block()
			{
				Thread thread = Thread.CurrentThread;

				lock (thread)
				{
					Monitor.Wait(thread);
				}
			}
		}

		protected class SampleThreadLocalCyclicDependencyGuard : ThreadStaticCyclicDependencyGuard
		{
			private Blocker blocker;

			public SampleThreadLocalCyclicDependencyGuard(Blocker blocker)
			{
				this.blocker = blocker;
			}

			public override object Run()
			{
				try
				{
					blocker.block();
				}
				catch
				{
					// ignore
				}
				return null;
			}
		}
	}
}