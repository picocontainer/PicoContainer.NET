using System;
using System.Collections;
using System.Security;
using PicoContainer;
using PicoContainer.Defaults;
using NUnit.Framework;
using PicoContainer.Tck;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class SetterInjectionComponentAdapterTestCase : AbstractComponentAdapterTestCase
	{
		protected override Type GetComponentAdapterType()
		{
			return typeof (SetterInjectionComponentAdapter);
		}

		protected override IComponentAdapterFactory CreateDefaultComponentAdapterFactory()
		{
			return new CachingComponentAdapterFactory(new SetterInjectionComponentAdapterFactory());
		}

		protected override IComponentAdapter prepDEF_verifyWithoutDependencyWorks(IMutablePicoContainer picoContainer)
		{
			return new SetterInjectionComponentAdapter(typeof (PersonBean), new IParameter[]
				{
					new ConstantParameter(
						"Pico Container")
				});
		}

		protected override IComponentAdapter prepDEF_verifyDoesNotInstantiate(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			return new SetterInjectionComponentAdapter(typeof (DeadBody), new IParameter[] {ComponentParameter.DEFAULT});
		}

		protected IComponentAdapter prepDEF_visitable()
		{
			return new SetterInjectionComponentAdapter(typeof (PersonBean), new IParameter[]
				{
					new ConstantParameter(
						"Pico Container")
				});
		}

		protected override IComponentAdapter prepSER_isSerializable(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			return new SetterInjectionComponentAdapter(typeof (PersonBean), new IParameter[] {ComponentParameter.DEFAULT});
		}

		protected IComponentAdapter prepDEF_isAbleToTakeParameters(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			picoContainer.RegisterComponentImplementation(typeof (PersonBean));
			return picoContainer.RegisterComponent(new SetterInjectionComponentAdapter(
				typeof (PurseBean), typeof (MoneyPurse), new IParameter[]
					{
						ComponentParameter.DEFAULT, new ConstantParameter(100.0D)
					}));
		}

		public class MoneyPurse : PurseBean
		{
			double money;

			public double Money
			{
				get { return money; }
				set { money = value; }
			}
		}

		protected override IComponentAdapter prepVER_verificationFails(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			picoContainer.RegisterComponentImplementation(typeof (PersonBean));
			return picoContainer.RegisterComponent(new SetterInjectionComponentAdapter(
				typeof (PurseBean), typeof (MoneyPurse), new IParameter[] {ComponentParameter.DEFAULT}));
		}

		protected override IComponentAdapter prepINS_createsNewInstances(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			return new SetterInjectionComponentAdapter(typeof (PersonBean), new IParameter[] {ComponentParameter.DEFAULT});
		}

		public class Ghost : PersonBean
		{
			public Ghost()
			{
				throw new VerificationException("test");
			}
		}

		protected override IComponentAdapter prepINS_errorIsRethrown(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			return new SetterInjectionComponentAdapter(typeof (Ghost), new IParameter[] {ComponentParameter.DEFAULT});
		}

		public class DeadBody
			: PersonBean
		{
			public DeadBody()
			{
				throw new SystemException("test");
			}
		}

		protected override IComponentAdapter prepINS_systemExceptionIsRethrown(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			return new SetterInjectionComponentAdapter(typeof (DeadBody), new IParameter[] {ComponentParameter.DEFAULT});
		}

		public class HidingPersion
			: PersonBean
		{
			public HidingPersion()
			{
				throw new Exception("test");
			}
		}

		protected override IComponentAdapter prepINS_normalExceptionIsRethrownInsidePicoInvocationTargetInitializationException(
			IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			return new SetterInjectionComponentAdapter(typeof (HidingPersion), new IParameter[] {ComponentParameter.DEFAULT});
		}

		protected override IComponentAdapter prepRES_dependenciesAreResolved(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			picoContainer.RegisterComponentImplementation(typeof (PersonBean));
			return new SetterInjectionComponentAdapter(typeof (PurseBean), new IParameter[] {ComponentParameter.DEFAULT});
		}

		public class WealthyPerson : PersonBean
		{
			PurseBean purse;

			public PurseBean Purse
			{
				get { return purse; }
				set { purse = value; }
			}
		}

		protected override IComponentAdapter prepRES_failingVerificationWithCyclicDependencyException(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			picoContainer.RegisterComponentImplementation(typeof (PersonBean), typeof (WealthyPerson));
			return picoContainer.RegisterComponent(new SetterInjectionComponentAdapter(
				typeof (PurseBean), new IParameter[] {ComponentParameter.DEFAULT}));
		}

		protected override IComponentAdapter prepRES_failingInstantiationWithCyclicDependencyException(IMutablePicoContainer picoContainer)
		{
			picoContainer.RegisterComponentInstance("Pico Container");
			picoContainer.RegisterComponentImplementation(typeof (PersonBean), typeof (WealthyPerson));
			return picoContainer.RegisterComponent(new SetterInjectionComponentAdapter(
				typeof (PurseBean), new IParameter[] {ComponentParameter.DEFAULT}));
		}

		public class A
		{
			private B b;
			private string stringValue;
			private IList list;

			public B B
			{
				get { return b; }
				set { b = value; }
			}

			public string StringValue
			{
				get { return stringValue; }
				set { stringValue = value; }
			}

			public IList List
			{
				get { return list; }
				set { list = value; }
			}
		}

		public class B
		{
		}

		[Test]
		public void AllUnsatisfiableDependenciesAreSignalled()
		{
			SetterInjectionComponentAdapter aAdapter = new SetterInjectionComponentAdapter("a", typeof (A));
			SetterInjectionComponentAdapter bAdapter = new SetterInjectionComponentAdapter("b", typeof (B));

			IMutablePicoContainer pico = new DefaultPicoContainer();
			pico.RegisterComponent(bAdapter);
			pico.RegisterComponent(aAdapter);

			try
			{
				aAdapter.GetComponentInstance(pico);
			}
			catch (UnsatisfiableDependenciesException e)
			{
				Assert.IsTrue(e.UnsatisfiableDependencies.Contains(typeof (IList)));
				Assert.IsTrue(e.UnsatisfiableDependencies.Contains(typeof (string)));
			}
		}

		public class C
		{
			private B b;
			private IList l;
			private bool asBean;

			public C()
			{
				asBean = true;
			}

			public C(B b)
			{
				this.l = new ArrayList();
				this.b = b;
				asBean = false;
			}

			public B B
			{
				get { return b; }
				set { b = value; }
			}

			public IList L
			{
				get { return l; }
				set { l = value; }
			}

			public bool instantiatedAsBean()
			{
				return asBean;
			}
		}

		[Test]
		public void HybridBeans()
		{
			SetterInjectionComponentAdapter bAdapter = new SetterInjectionComponentAdapter("b", typeof (B));
			SetterInjectionComponentAdapter cAdapter = new SetterInjectionComponentAdapter("c", typeof (C));
			SetterInjectionComponentAdapter cNullAdapter = new SetterInjectionComponentAdapter("c0", typeof (C));

			IMutablePicoContainer pico = new DefaultPicoContainer();
			pico.RegisterComponent(bAdapter);
			pico.RegisterComponent(cAdapter);
			pico.RegisterComponent(cNullAdapter);
			pico.RegisterComponentImplementation(typeof (ArrayList));

			C c = (C) cAdapter.GetComponentInstance(pico);
			Assert.IsTrue(c.instantiatedAsBean());
			C c0 = (C) cNullAdapter.GetComponentInstance(pico);
			Assert.IsTrue(c0.instantiatedAsBean());
		}

		/*public class Yin {
		private Yang yang;

		public void setYin(Yang yang) {
			this.yang = yang;
		}

		public Yang getYang() {
			return yang;
		}
	}

	public class Yang {
		private Yin yin;

		public void setYang(Yin yin) {
			this.yin = yin;
		}

		public Yin getYin() {
			return yin;
		}
	}

	// TODO PICO-188
	// http://jira.codehaus.org/browse/PICO-188
	public void FIXME_testShouldBeAbleToHandleMutualDependenciesWithSetterInjection() {
		MutablePicoContainer pico = new DefaultPicoContainer(new CachingComponentAdapterFactory(
				new SetterInjectionComponentAdapterFactory()));

		pico.RegisterComponentImplementation(Yin.class);
		pico.RegisterComponentImplementation(Yang.class);

		Yin yin = (Yin) pico.getComponentInstance(Yin.class);
		Yang yang = (Yang) pico.getComponentInstance(Yang.class);

		assertSame(yin, yang.getYin());
		assertSame(yang, yin.getYang());
	}*/
	}
}