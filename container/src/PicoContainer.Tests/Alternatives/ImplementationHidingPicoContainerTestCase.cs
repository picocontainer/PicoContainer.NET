using System.Collections;
using NUnit.Framework;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
    [TestFixture]
    public class ImplementationHidingPicoContainerTestCase : AbstractImplementationHidingPicoContainerTestCase
    {
        protected override IMutablePicoContainer CreateImplementationHidingPicoContainer()
        {
            return new ImplementationHidingPicoContainer();
        }

        protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent,
                                                                     ILifecycleManager lifecycleManager)
        {
            return new ImplementationHidingPicoContainer(new DefaultComponentAdapterFactory(), parent, lifecycleManager);
        }

        protected override IMutablePicoContainer CreatePicoContainer(IPicoContainer parent)
        {
            return new ImplementationHidingPicoContainer(parent);
        }

        [Test]
        public void UsageOfADifferentComponentAdapterFactory()
        {
            // Jira bug 212
            IMutablePicoContainer parent = new DefaultPicoContainer();
            ImplementationHidingPicoContainer pico =
                new ImplementationHidingPicoContainer(new ConstructorInjectionComponentAdapterFactory(), parent);
            pico.RegisterComponentImplementation(typeof (IList), typeof (ArrayList));

            IList list1 = (IList) pico.GetComponentInstanceOfType(typeof (IList));

            IList list2 = (IList) pico.GetComponentInstanceOfType(typeof (IList));

            Assert.IsNotNull(list1);
            Assert.IsNotNull(list2);
            Assert.IsFalse(list1 == list2);
        }

        /*public static class MyThread : Thread
		{
			public MyThread(String s)
			{
				super(s);
			}
		}*/

        /*public void testHidingWithoutParameter() 
		{
			// this was a bug reported by Arnd Kors on 21st Sept on the mail list.
			ImplementationHidingPicoContainer pico = new ImplementationHidingPicoContainer();
			pico.RegisterComponentImplementation(typeof(string));

			pico.RegisterComponentImplementation(Runnable.class , MyThread. class );

			new VerifyingVisitor().traverse(pico);
		}*/
    }
}