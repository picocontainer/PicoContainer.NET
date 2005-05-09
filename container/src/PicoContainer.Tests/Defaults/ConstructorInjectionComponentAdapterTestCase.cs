using System;
using System.Collections;
using System.Security;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.Tck;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class ConstructorInjectionComponentAdapterTestCase : AbstractComponentAdapterTestCase
	{
		protected override Type GetComponentAdapterType() 
		{
			return typeof(ConstructorInjectionComponentAdapter);
		}

		protected override IComponentAdapter prepDEF_verifyWithoutDependencyWorks(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter("foo", typeof (A));
		}

		public class A
		{
			public A()
			{
				Assert.Fail("verification should not instantiate");
			}
		}

		public class B
		{
			public B(A a)
			{
				Assert.Fail("verification should not instantiate");
			}
		}

		protected override IComponentAdapter prepDEF_verifyDoesNotInstantiate(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentImplementation(typeof (A));
			return new ConstructorInjectionComponentAdapter(typeof (B), typeof (B));
		}

		protected IComponentAdapter prepDEF_visitable()
		{
			return new ConstructorInjectionComponentAdapter("bar", typeof (B), new IParameter[] {ComponentParameter.DEFAULT});
		}

		protected IComponentAdapter prepDEF_isAbleToTakeParameters(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentImplementation(typeof (SimpleTouchable));
			return new ConstructorInjectionComponentAdapter(typeof (NamedDependsOnTouchable), typeof (NamedDependsOnTouchable), new IParameter[]
				{
					ComponentParameter.DEFAULT, new ConstantParameter("Name")
				});
		}

		protected override IComponentAdapter prepSER_isSerializable(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter(typeof (SimpleTouchable), typeof (SimpleTouchable));
		}

		public class NamedDependsOnTouchable
			: DependsOnTouchable
		{
			public NamedDependsOnTouchable(ITouchable t, String name) : base(t)
			{
			}
		}

		protected override IComponentAdapter prepVER_verificationFails(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter(typeof (DependsOnTouchable), typeof (DependsOnTouchable));
		}

		protected override IComponentAdapter prepINS_createsNewInstances(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter(typeof (SimpleTouchable), typeof (SimpleTouchable));
		}

		public class Erroneous
		{
			public Erroneous()
			{
				throw new VerificationException("test");
			}
		}

		protected override IComponentAdapter prepINS_errorIsRethrown(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter(typeof (Erroneous), typeof (Erroneous));
		}

		public class RuntimeThrowing
		{
			public RuntimeThrowing()
			{
				throw new SystemException("test");
			}
		}

		protected override IComponentAdapter prepINS_systemExceptionIsRethrown(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter(typeof (RuntimeThrowing), typeof (RuntimeThrowing));
		}


		public class NormalExceptionThrowing
		{
			public NormalExceptionThrowing()
			{
				throw new Exception("test");
			}
		}

		protected override IComponentAdapter prepINS_normalExceptionIsRethrownInsidePicoInvocationTargetInitializationException(IMutablePicoContainer picoContainer)
		{
			return new ConstructorInjectionComponentAdapter(typeof (NormalExceptionThrowing), typeof (NormalExceptionThrowing));
		}

		protected override IComponentAdapter prepRES_dependenciesAreResolved(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentImplementation(typeof (SimpleTouchable));
			return new ConstructorInjectionComponentAdapter(typeof (DependsOnTouchable), typeof (DependsOnTouchable));
		}

		public class C1
		{
			public C1(C2 c2)
			{
				Assert.Fail("verification should not instantiate");
			}
		}

		public class C2
		{
			public C2(C1 c1)
			{
				Assert.Fail("verification should not instantiate");
			}
		}

		protected override IComponentAdapter prepRES_failingVerificationWithCyclicDependencyException(IMutablePicoContainer picoContainer)
		{
			IComponentAdapter componentAdapter = new ConstructorInjectionComponentAdapter(typeof (C1), typeof (C1));
			picoContainer.RegisterComponent(componentAdapter);
			picoContainer.RegisterComponentImplementation(typeof (C2), typeof (C2));
			return componentAdapter;
		}

		protected override IComponentAdapter prepRES_failingInstantiationWithCyclicDependencyException(IMutablePicoContainer picoContainer)
		{
			IComponentAdapter componentAdapter = new ConstructorInjectionComponentAdapter(typeof (C1), typeof (C1));
			picoContainer.RegisterComponent(componentAdapter);
			picoContainer.RegisterComponentImplementation(typeof (C2), typeof (C2));
			return componentAdapter;
		}

		[Test]
		public void NormalExceptionThrownInCtorIsRethrownInsideInvocationTargetExeption()
		{
			DefaultPicoContainer picoContainer = new DefaultPicoContainer();
			picoContainer.RegisterComponentImplementation(typeof (NormalExceptionThrowing));
			try
			{
				picoContainer.GetComponentInstance(typeof (NormalExceptionThrowing));
				Assert.Fail();
			}
			catch (PicoInvocationTargetInitializationException e)
			{
				Assert.AreEqual("test", e.GetBaseException().Message);
			}
		}

		public abstract class InstantiationExceptionThrowing
		{
			public InstantiationExceptionThrowing()
			{
			}
		}

		[Test]
		[ExpectedException(typeof (NotConcreteRegistrationException))]
		public void InstantiationExceptionThrownInCtorIsRethrownInsideInvocationTargetExeption()
		{
			DefaultPicoContainer picoContainer = new DefaultPicoContainer();
			picoContainer.RegisterComponentImplementation(typeof (InstantiationExceptionThrowing));
			picoContainer.GetComponentInstance(typeof (InstantiationExceptionThrowing));
		}

		public class IllegalAccessExceptionThrowing
		{
			private IllegalAccessExceptionThrowing()
			{
			}
		}

		[Test]
		public void PicoInitializationExceptionThrownBecauseOfFilteredConstructors()
		{
			DefaultPicoContainer picoContainer = new DefaultPicoContainer();
			try
			{
				picoContainer.RegisterComponentImplementation(typeof (IllegalAccessExceptionThrowing));
				picoContainer.GetComponentInstance(typeof (IllegalAccessExceptionThrowing));
				Assert.Fail();
			}
			catch (PicoInitializationException e)
			{
				Assert.IsTrue(e.Message.IndexOf(typeof (IllegalAccessExceptionThrowing).Name) > 0);
			}
		}

		[Test]
		public void RegisterAbstractShouldFail()
		{
			IMutablePicoContainer pico = new DefaultPicoContainer();

			try
			{
				pico.RegisterComponentImplementation(typeof (IList));
				Assert.Fail("Shouldn't be allowed to register abstract classes or interfaces.");
			}
			catch (NotConcreteRegistrationException e)
			{
				Assert.AreEqual(typeof (IList), e.ComponentImplementation);
				Assert.IsTrue(e.Message.IndexOf(typeof (IList).Name) > 0);
			}
		}

		private class Private
		{
			private Private()
			{
			}
		}

		private class NotYourBusiness
		{
			private NotYourBusiness(Private aPrivate)
			{
				Assert.IsNotNull(aPrivate);
			}
		}

		// http://jira.codehaus.org/browse/PICO-189
		[Test]
		public void ShouldBeAbleToInstantiateNonPublicClassesWithNonPublicConstructors()
		{
			DefaultPicoContainer pico = new DefaultPicoContainer(new ConstructorInjectionComponentAdapterFactory(true));
			pico.RegisterComponentImplementation(typeof (Private));
			pico.RegisterComponentImplementation(typeof (NotYourBusiness));
			Assert.IsNotNull(pico.GetComponentInstance(typeof (NotYourBusiness)));
		}

		public class Component201
		{
			public Component201(String s)
			{
			}

			protected Component201(int i, bool b)
			{
				Assert.Fail("Wrong constructor taken.");
			}
		}

		// http://jira.codehaus.org/browse/PICO-201
		[Test]
		public void ShouldNotConsiderNonPublicConstructors()
		{
			DefaultPicoContainer pico = new DefaultPicoContainer();
			pico.RegisterComponentImplementation(typeof (Component201));
			pico.RegisterComponentInstance(2);
			pico.RegisterComponentInstance(true);
			pico.RegisterComponentInstance("Hello");
			Assert.IsNotNull(pico.GetComponentInstance(typeof (Component201)));
		}

		/*public void testMonitoringHappensBeforeAndAfterInstantiation() {
		long beforeTime = System.currentTimeMillis();

		Mock monitor = mock(ComponentMonitor.class);
		Constructor emptyHashMapCtor = HashMap.class.getConstructor(new Class[0]);
		monitor.expects(once()).method("instantiating").with(eq(emptyHashMapCtor));
		Constraint startIsAfterBegin = new Constraint() {
			public boolean eval(Object o) {
				Long startTime = (Long) o;
				return beforeTime <= startTime.longValue();
			}

			public StringBuffer describeTo(StringBuffer stringBuffer) {
				return stringBuffer.append("The startTime wasn't after the begin of the test");
			}
		};
		Constraint durationIsGreaterThanOrEqualToZero = new Constraint() {
			public boolean eval(Object o) {
				Long duration = (Long) o;
				return 0 <= duration.longValue();
			}

			public StringBuffer describeTo(StringBuffer stringBuffer) {
				return stringBuffer.append("The endTime wasn't after the startTime");
			}
		};

		monitor.expects(once()).method("instantiated").with(eq(emptyHashMapCtor), startIsAfterBegin, durationIsGreaterThanOrEqualToZero);
		ConstructorInjectionComponentAdapter cica = new ConstructorInjectionComponentAdapter(Map.class, HashMap.class,
				new Parameter[0], false, (ComponentMonitor) monitor.proxy());
		cica.getComponentInstance(null);
	}

	public void testMonitoringHappensBeforeAndOnFailOfImpossibleComponentsInstantiation() throws NoSuchMethodException {
		final long beforeTime = System.currentTimeMillis();

		Mock monitor = mock(ComponentMonitor.class);
		Constructor barfingActionListenerCtor = BarfingActionListener.class.getConstructor(new Class[0]);
		monitor.expects(once()).method("instantiating").with(eq(barfingActionListenerCtor));

		Constraint isITE = new Constraint() {
			public boolean eval(Object o) {
				Exception ex = (Exception) o;
				return ex instanceof InvocationTargetException;
			}

			public StringBuffer describeTo(StringBuffer stringBuffer) {
				return stringBuffer.append("Should have been unable to instantiate");
			}
		};

		monitor.expects(once()).method("instantiationFailed").with(eq(barfingActionListenerCtor), isITE);
		ConstructorInjectionComponentAdapter cica = new ConstructorInjectionComponentAdapter(ActionListener.class, BarfingActionListener.class,
				new Parameter[0], false, (ComponentMonitor) monitor.proxy());
		try {
			cica.getComponentInstance(null);
			fail("Should barf");
		} catch (RuntimeException e) {
			assertEquals("Barf!", e.getMessage());
		}
	}

	private class BarfingActionListener : ActionListener {
		public BarfingActionListener() {
			throw new RuntimeException("Barf!");
		}

		public void actionPerformed(ActionEvent e) {
		}
	}*/
	}
}