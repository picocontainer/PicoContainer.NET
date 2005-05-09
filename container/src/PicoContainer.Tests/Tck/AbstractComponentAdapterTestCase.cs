using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;

namespace PicoContainer.Tck
{
	[TestFixture]
	public abstract class AbstractComponentAdapterTestCase
	{
		public static int SERIALIZABLE = 1;
		public static int VERIFYING = 2;
		public static int INSTANTIATING = 4;
		public static int RESOLVING = 8;

		protected abstract Type GetComponentAdapterType();

		protected virtual int GetComponentAdapterNature() 
		{
			return SERIALIZABLE | VERIFYING | INSTANTIATING | RESOLVING;
		}

		protected virtual IComponentAdapterFactory CreateDefaultComponentAdapterFactory() 
		{
			return new DefaultComponentAdapterFactory();
		}

		protected virtual IPicoContainer WrapComponentInstances(Type decoratingComponentAdapterClass, IPicoContainer picoContainer, object[] wrapperDependencies)
		{
			Assert.IsTrue(typeof (DecoratingComponentAdapter).IsAssignableFrom(decoratingComponentAdapterClass));
			IMutablePicoContainer mutablePicoContainer = new DefaultPicoContainer();
			int size = (wrapperDependencies != null ? wrapperDependencies.Length : 0) + 1;
			ICollection allComponentAdapters = picoContainer.ComponentAdapters;

			foreach (object adapter in allComponentAdapters)
			{
				IParameter[] parameters = new IParameter[size];
				parameters[0] = new ConstantParameter(adapter);
				for (int i = 1; i < parameters.Length; i++)
				{
					parameters[i] = new ConstantParameter(wrapperDependencies[i - 1]);
				}
				IMutablePicoContainer instantiatingPicoContainer = new DefaultPicoContainer(new ConstructorInjectionComponentAdapterFactory());
				instantiatingPicoContainer.RegisterComponentImplementation("decorator", decoratingComponentAdapterClass, parameters);
				mutablePicoContainer.RegisterComponent((IComponentAdapter) instantiatingPicoContainer.GetComponentInstance("decorator"));
			}
			return mutablePicoContainer;
		}

		/// <summary>
		/// Prepare the test VerifyWithoutDependencyWorks																																 */
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test for a component without dependencies. Registration in the pico is not necessary.</returns>
		protected abstract IComponentAdapter prepDEF_verifyWithoutDependencyWorks(IMutablePicoContainer picoContainer);

		[Test]
		public void DEF_verifyWithoutDependencyWorks()
		{
			IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
			IComponentAdapter componentAdapter = prepDEF_verifyWithoutDependencyWorks(picoContainer);
			Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
			componentAdapter.Verify(picoContainer);
		}

		/// <summary>
		/// Prepare the test <em>verifyDoesNotInstantiate</em>.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test for a component that may throw on instantiation.
		/// Registration in the pico is not necessary.</returns>
		protected abstract IComponentAdapter prepDEF_verifyDoesNotInstantiate(IMutablePicoContainer picoContainer);

		[Test]
		public void DEF_verifyDoesNotInstantiate()
		{
			IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
			IComponentAdapter componentAdapter = prepDEF_verifyDoesNotInstantiate(picoContainer);
			Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
			IComponentAdapter notInstantiatablecomponentAdapter = new NotInstantiatableComponentAdapter(componentAdapter);
			IPicoContainer wrappedPicoContainer = WrapComponentInstances(typeof (NotInstantiatableComponentAdapter), picoContainer, null);
			notInstantiatablecomponentAdapter.Verify(wrappedPicoContainer);
		}

		/** 
		 * NOTE mward skipped following method from Java because visitor is not implemented
		 * 
		 * protected abstract ComponentAdapter prepDEF_visitable();
		 * public void testDEF_visitable() 
		 * protected ComponentAdapter prepDEF_isAbleToTakeParameters(IMutablePicoContainer picoContainer)
		 * public void testDEF_isAbleToTakeParameters()
		 **/

		#region Serializable Tests

		/// <summary>
		/// Prepare the test <em>isSerializable</em>. Overload this function, if the ComponentAdapter supports
		/// serialization.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test. Registration in the pico is not necessary.</returns>
		protected virtual IComponentAdapter prepSER_isSerializable(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void SER_isSerializable()
		{
			if ((GetComponentAdapterNature() & SERIALIZABLE) > 0)
			{
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepSER_isSerializable(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				Object instance = componentAdapter.GetComponentInstance(picoContainer);
				Assert.IsNotNull(instance);

				IComponentAdapter serializedComponentAdapter = null;

				using (Stream stream = new MemoryStream())
				{
					// Serialize it to memory
					IFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, componentAdapter);

					// De-Serialize from memory
					stream.Seek(0, 0); // reset stream to begining
					serializedComponentAdapter = (IComponentAdapter) formatter.Deserialize(stream);
				}

				Assert.AreEqual(componentAdapter.ComponentKey, serializedComponentAdapter.ComponentKey);
				Object instanceAfterSerialization = serializedComponentAdapter.GetComponentInstance(picoContainer);
				Assert.IsNotNull(instanceAfterSerialization);
				Assert.AreSame(instance.GetType(), instanceAfterSerialization.GetType());
			}
		}

		#endregion

		#region Verifying Tests

		/// <summary>
		/// Prepare the test <em>VerificationFailsWithUnsatisfiedDependency</em>. Overload this function, if the
		/// IComponentAdapter's verification can fail e.g. due to an unresolved dependency.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test, that fails for the verification, e.g. because of a compoennt 
		/// with missing dependencies. Registration in the pico is not necessary.</returns>
		protected virtual IComponentAdapter prepVER_verificationFails(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void VER_verificationFails()
		{
			if ((GetComponentAdapterNature() & VERIFYING) > 0)
			{
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepVER_verificationFails(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				try
				{
					componentAdapter.Verify(picoContainer);
					Assert.Fail("PicoIntrospectionException expected");
				}
				catch (PicoIntrospectionException)
				{
				}
				catch (Exception e)
				{
					Assert.Fail("PicoIntrospectionException expected, but got " + e.GetType().Name);
				}
				try
				{
					componentAdapter.GetComponentInstance(picoContainer);
					Assert.Fail("PicoInitializationException or PicoIntrospectionException expected");
				}
				catch (PicoInitializationException)
				{
				}
				catch (PicoIntrospectionException)
				{
				}
				catch (Exception e)
				{
					Assert.Fail("PicoInitializationException or PicoIntrospectionException expected, but got " + e.GetType().Name);
				}
			}
		}
		
		#endregion

		#region Instantiating Tests

		/// <summary>
		/// Prepare the test <em>createsNewInstances</em>. Overload this function, if the ComponentAdapter is instantiating. It
		/// should create a new instance with every call.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test. Registration in the pico is not necessary.</returns>
		protected virtual IComponentAdapter prepINS_createsNewInstances(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void INS_createsNewInstances()
		{
			if ((GetComponentAdapterNature() & INSTANTIATING) > 0)
			{
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepINS_createsNewInstances(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				object instance = componentAdapter.GetComponentInstance(picoContainer);
				Assert.IsNotNull(instance);
				// following was a "AreNotSame" assertion
				Assert.IsFalse(instance == componentAdapter.GetComponentInstance(picoContainer));
				Assert.AreSame(instance.GetType(), componentAdapter.GetComponentInstance(picoContainer).GetType());
			}
		}

		/// <summary>
		/// Prepare the test <em>errorIsRethrown</em>. Overload this function, if the ComponentAdapter is instantiating.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test with a component that fails with an {@link Error}at instantiation.
		/// Registration in the pico is not necessary.</returns>
		protected virtual IComponentAdapter prepINS_errorIsRethrown(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void INS_errorIsRethrown()
		{
			if ((GetComponentAdapterNature() & INSTANTIATING) > 0)
			{
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepINS_errorIsRethrown(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());

				try
				{
					componentAdapter.GetComponentInstance(picoContainer);
					Assert.Fail("Thrown Error excpected");
				}
				catch (Exception e)
				{
					Assert.AreEqual("test", e.GetBaseException().Message);
				}
			}
		}

		/// <summary>
		/// Prepare the test <em>runtimeExceptionIsRethrown</em>. Overload this function, if the ComponentAdapter is
		/// instantiating.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test with a component that fails with a SystemException at
		/// instantiation. Registration in the pico is not necessary.</returns>
		protected virtual IComponentAdapter prepINS_systemExceptionIsRethrown(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void INS_runtimeExceptionIsRethrown()
		{
			if ((GetComponentAdapterNature() & INSTANTIATING) > 0)
			{
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepINS_systemExceptionIsRethrown(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				try
				{
					componentAdapter.GetComponentInstance(picoContainer);
					Assert.Fail("Thrown RuntimeException excpected");
				}
				catch (PicoInvocationTargetInitializationException e)
				{
					Assert.AreEqual("test", e.GetBaseException().Message);
				}
			}
		}

		/// <summary>
		/// Prepare the test <em>normalExceptionIsRethrownInsidePicoInvocationTargetInitializationException</em>. Overload this
		/// function, if the ComponentAdapter is instantiating.
		/// </summary>
		/// <param name="picoContainer">container, may probably not be used.</param>
		/// <returns>a ComponentAdapter of the type to test with a component that fails with a
		/// PicoInvocationTargetInitializationException at instantiation. Registration in the pico is not necessary.</returns>
		protected virtual IComponentAdapter prepINS_normalExceptionIsRethrownInsidePicoInvocationTargetInitializationException(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void INS_normalExceptionIsRethrownInsidePicoInvocationTargetInitializationException()
		{
			if ((GetComponentAdapterNature() & INSTANTIATING) > 0)
			{
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepINS_normalExceptionIsRethrownInsidePicoInvocationTargetInitializationException(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				try
				{
					componentAdapter.GetComponentInstance(picoContainer);
					Assert.Fail("Thrown PicoInvocationTargetInitializationException excpected");
				}
				catch (PicoInvocationTargetInitializationException e)
				{
					Assert.IsTrue(e.GetBaseException().Message.EndsWith("test"));
				}
			}
		}

		#endregion

		#region Resolving Tests

		/// <summary>
		/// Prepare the test <em>dependenciesAreResolved</em>. Overload this function, if the ComponentAdapter is resolves
		/// dependencies.
		/// </summary>
		/// <param name="picoContainer">container, used to register dependencies.</param>
		/// <returns>a ComponentAdapter of the type to test with a component that has dependencies. Registration in the 
		/// pico is not necessary.</returns>
		protected virtual IComponentAdapter prepRES_dependenciesAreResolved(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void RES_dependenciesAreResolved()
		{
			if ((GetComponentAdapterNature() & RESOLVING) > 0)
			{
				IList dependencies = new ArrayList();
				object[] wrapperDependencies = new object[] {dependencies};
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepRES_dependenciesAreResolved(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				Assert.IsFalse(picoContainer.ComponentAdapters.Contains(componentAdapter));
				IPicoContainer wrappedPicoContainer = WrapComponentInstances(typeof (CollectingComponentAdapter), picoContainer, wrapperDependencies);
				object instance = componentAdapter.GetComponentInstance(wrappedPicoContainer);
				Assert.IsNotNull(instance);
				Assert.IsTrue(dependencies.Count > 0);
			}
		}

		/// <summary>
		/// Prepare the test <em>failingVerificationWithCyclicDependencyException</em>. Overload this function, if the
		/// ComponentAdapter is resolves dependencies.
		/// </summary>
		/// <param name="picoContainer">container, used to register dependencies.</param>
		/// <returns>a ComponentAdapter of the type to test with a component that has cyclic dependencies. You have to register the
		/// component itself in the pico.</returns>
		protected virtual IComponentAdapter prepRES_failingVerificationWithCyclicDependencyException(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void RES_failingVerificationWithCyclicDependencyException()
		{
			if ((GetComponentAdapterNature() & RESOLVING) > 0)
			{
				Hashtable cycleInstances = new Hashtable();
				PicoContainer.Defaults.IObjectReference cycleCheck = new SimpleReference();
				object[] wrapperDependencies = new Object[] {cycleInstances, cycleCheck};
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepRES_failingVerificationWithCyclicDependencyException(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				Assert.IsTrue(picoContainer.ComponentAdapters.Contains(componentAdapter));
				IPicoContainer wrappedPicoContainer = WrapComponentInstances(typeof (CycleDetectorComponentAdapter), picoContainer, wrapperDependencies);
				try
				{
					componentAdapter.Verify(wrappedPicoContainer);
					Assert.Fail("Thrown PicoVerificationException excpected");
				}
				catch (CyclicDependencyException cycle)
				{
					object[] dependencies = cycle.Dependencies;
					Assert.AreSame(dependencies[0], dependencies[dependencies.Length - 1]);
				}
			}
		}

		/// <summary>
		/// Prepare the test <em>failingInstantiationWithCyclicDependencyException</em>. Overload this function, if the
		/// ComponentAdapter is resolves dependencies.
		/// </summary>
		/// <param name="picoContainer">container, used to register dependencies.</param>
		/// <returns>a ComponentAdapter of the type to test with a component that has cyclic dependencies. You have to register the
		/// component itself in the pico.</returns>
		protected virtual IComponentAdapter prepRES_failingInstantiationWithCyclicDependencyException(IMutablePicoContainer picoContainer)
		{
			throw new AssertionException("You have to overwrite this method for a useful test");
		}

		[Test]
		public void RES_failingInstantiationWithCyclicDependencyException()
		{
			if ((GetComponentAdapterNature() & RESOLVING) > 0)
			{
				Hashtable cycleInstances = new Hashtable();
				PicoContainer.Defaults.IObjectReference cycleCheck = new SimpleReference();
				object[] wrapperDependencies = new Object[] {cycleInstances, cycleCheck};
				IMutablePicoContainer picoContainer = new DefaultPicoContainer(CreateDefaultComponentAdapterFactory());
				IComponentAdapter componentAdapter = prepRES_failingInstantiationWithCyclicDependencyException(picoContainer);
				Assert.AreSame(GetComponentAdapterType(), componentAdapter.GetType());
				Assert.IsTrue(picoContainer.ComponentAdapters.Contains(componentAdapter));
				IPicoContainer wrappedPicoContainer = WrapComponentInstances(typeof (CycleDetectorComponentAdapter), picoContainer, wrapperDependencies);
				try
				{
					componentAdapter.GetComponentInstance(wrappedPicoContainer);
					Assert.Fail("Thrown CyclicDependencyException excpected");
				}
				catch (CyclicDependencyException e)
				{
					object[] dependencies = e.Dependencies;
					Assert.AreSame(dependencies[0], dependencies[dependencies.Length - 1]);
				}
			}
		}

		#endregion
	}

	#region Model & Helpers

	public class NotInstantiatableComponentAdapter : DecoratingComponentAdapter
	{
		public NotInstantiatableComponentAdapter(IComponentAdapter componentAdapter) : base(componentAdapter)
		{
		}

		public Object getComponentInstance(IPicoContainer container)
		{
			Assert.Fail("Not instantiatable");
			return null;
		}
	}

	public class CollectingComponentAdapter : DecoratingComponentAdapter
	{
		IList list;

		public CollectingComponentAdapter(IComponentAdapter theDelegate, IList list) : base(theDelegate)
		{
			this.list = list;
		}

		public override object GetComponentInstance(IPicoContainer container)
		{
			Object result = base.GetComponentInstance(container);
			list.Add(result);
			return result;
		}
	}

	public class CycleDetectorComponentAdapter : DecoratingComponentAdapter
	{
		private Hashtable set;
		private PicoContainer.Defaults.IObjectReference reference;

		public CycleDetectorComponentAdapter(IComponentAdapter theDelegate,
		                                     Hashtable set,
		                                     PicoContainer.Defaults.IObjectReference reference) : base(theDelegate)
		{
			this.set = set;
			this.reference = reference;
		}

		public Object getComponentInstance(IPicoContainer container)
		{
			if (set.Contains(this))
			{
				reference.Set(this);
			}
			else
			{
				set.Add(this, this);
			}
			return base.GetComponentInstance(container);
		}
	}

	#endregion
}
