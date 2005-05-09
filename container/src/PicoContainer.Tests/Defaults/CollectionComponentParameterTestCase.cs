using System;
using System.Collections;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class CollectionComponentParameterTestCase
	{
		private IMutablePicoContainer GetDefaultPicoContainer()
		{
			IMutablePicoContainer mpc = new DefaultPicoContainer();
			mpc.RegisterComponentImplementation(typeof (Bowl));
			mpc.RegisterComponentImplementation(typeof (Cod));
			mpc.RegisterComponentImplementation(typeof (Shark));
			return mpc;
		}

		[Test]
		public void ShouldInstantiateArrayOfStrings()
		{
			CollectionComponentParameter ccp = new CollectionComponentParameter();

			Mock componentAdapterMock = new DynamicMock(typeof (IComponentAdapter));
			componentAdapterMock.ExpectAndReturn("ComponentKey", "x", null);
			componentAdapterMock.ExpectAndReturn("ComponentKey", "x", null);

			Mock containerMock = new DynamicMock(typeof (IPicoContainer));
			containerMock.ExpectAndReturn("ComponentAdapters", new Hashtable(), null);

			IComponentAdapter[] adapters = new IComponentAdapter[]
				{
					new InstanceComponentAdapter("y", "Hello"),
					new InstanceComponentAdapter("z", "World")
				};

			containerMock.ExpectAndReturn("GetComponentAdaptersOfType", adapters, new IsEqual(typeof (string)));
			containerMock.ExpectAndReturn("GetComponentInstance", "World", new IsEqual("z"));
			containerMock.ExpectAndReturn("GetComponentInstance", "Hello", new IsEqual("y"));
			containerMock.ExpectAndReturn("Parent", null, null);

			ArrayList expected = new ArrayList(new string[] {"Hello", "World"});
			expected.Sort();

			object[] array = (object[]) ccp.ResolveInstance((IPicoContainer) containerMock.MockInstance,
                (IComponentAdapter) componentAdapterMock.MockInstance, 
				typeof (string[]));

			ArrayList actual = new ArrayList(array);
			actual.Sort();
			Assert.AreEqual(expected.ToArray(), actual.ToArray());

			// Verify mocks
			componentAdapterMock.Verify();
			containerMock.Verify();
		}

		[Test]
		public void NativeArrays()
		{
			IMutablePicoContainer mpc = GetDefaultPicoContainer();
			Cod cod = (Cod) mpc.GetComponentInstanceOfType(typeof (Cod));
			Bowl bowl = (Bowl) mpc.GetComponentInstance(typeof (Bowl));
			Assert.AreEqual(1, bowl.cods.Length);
			Assert.AreEqual(2, bowl.fishes.Length);
			Assert.AreSame(cod, bowl.cods[0]);

			try
			{
				Assert.AreSame(bowl.fishes[0], bowl.fishes[1]);
				Assert.Fail("fish should not be the same");
			}
			catch (AssertionException)
			{
			}
		}

		[Test]
		public void CollectionsAreGeneratedOnTheFly()
		{
			IMutablePicoContainer mpc = new DefaultPicoContainer();
			mpc.RegisterComponent(new ConstructorInjectionComponentAdapter(typeof (Bowl), typeof (Bowl)));
			mpc.RegisterComponentImplementation(typeof (Cod));
			Bowl bowl = (Bowl) mpc.GetComponentInstance(typeof (Bowl));
			Assert.AreEqual(1, bowl.cods.Length);
			mpc.RegisterComponentInstance("Nemo", new Cod());
			bowl = (Bowl) mpc.GetComponentInstance(typeof (Bowl));
			Assert.AreEqual(2, bowl.cods.Length);

			try
			{
				Assert.AreSame(bowl.cods[0], bowl.cods[1]);
				Assert.Fail("cods should not be the same");
			}
			catch (AssertionException)
			{
			}
		}

		[Test]
		public void TestCollections()
		{
			IMutablePicoContainer mpc = new DefaultPicoContainer();
			IParameter[] parameters = new IParameter[]{new ComponentParameter(typeof(Cod), false), 
														  new ComponentParameter(typeof(Fish), false)};

			mpc.RegisterComponentImplementation(typeof(CollectedBowl), typeof(CollectedBowl), parameters);
			mpc.RegisterComponentImplementation(typeof(Cod));
			mpc.RegisterComponentImplementation(typeof(Shark));
			Cod cod = (Cod) mpc.GetComponentInstanceOfType(typeof(Cod));
			CollectedBowl bowl = (CollectedBowl) mpc.GetComponentInstance(typeof(CollectedBowl));
			Assert.AreEqual(1, bowl.cods.Length);
			Assert.AreEqual(2, bowl.fishes.Length);
			Assert.AreSame(cod, bowl.cods[0]);

			try
			{
				Assert.AreSame(bowl.fishes[0], bowl.fishes[1]);
				Assert.Fail("The fishes should not be the same");
			}
			catch(AssertionException) {}
		}

		public class DictionaryBowl 
		{
			private Fish[] fishes;

			public Fish[] Fishes
			{
				get { return fishes; }
			}

			public DictionaryBowl(IDictionary dictionary) 
			{
				ArrayList list = new ArrayList(dictionary.Values);
				this.fishes = (Fish[]) list.ToArray(typeof(Fish));
			}
		}

		[Test]
		public void TestDictionaries()
		{
			IMutablePicoContainer mpc = new DefaultPicoContainer();
			IParameter[] parameters = new IParameter[] {new ComponentParameter(typeof (Fish), false)};

			mpc.RegisterComponentImplementation(typeof (DictionaryBowl), typeof (DictionaryBowl), parameters);
			mpc.RegisterComponentImplementation(typeof (Cod));
			mpc.RegisterComponentImplementation(typeof (Shark));
			DictionaryBowl bowl = (DictionaryBowl) mpc.GetComponentInstance(typeof (DictionaryBowl));
			Assert.AreEqual(2, bowl.Fishes.Length);

			try
			{
				Assert.AreSame(bowl.Fishes[0], bowl.Fishes[1]);
				Assert.Fail("Should not be the same fish");
			} 
			catch(AssertionException) {}
		}

		public class UngenericCollectionBowl
		{
			public UngenericCollectionBowl(ICollection fish) 
			{
			}
		}

		[Test]
		[ExpectedException(typeof(UnsatisfiableDependenciesException))]
		public void ShouldNotInstantiateCollectionForUngenericCollectionParameters() 
		{
			IMutablePicoContainer pico = GetDefaultPicoContainer();
			pico.RegisterComponentImplementation(typeof(UngenericCollectionBowl));		
			pico.GetComponentInstance(typeof(UngenericCollectionBowl));
		}

		public class AnotherGenericCollectionBowl
		{
			private string[] strings;

			public AnotherGenericCollectionBowl(string[] strings)
			{
				this.strings = strings;
			}

			public string[] Strings
			{
				get { return strings; }
			}
		}

		[Test]
		[ExpectedException(typeof(UnsatisfiableDependenciesException))]
		public void ShouldFailWhenThereAreNoComponentsToPutInTheArray()
		{
			IMutablePicoContainer pico = GetDefaultPicoContainer();
			pico.RegisterComponentImplementation(typeof (AnotherGenericCollectionBowl));
			pico.GetComponentInstance(typeof (AnotherGenericCollectionBowl));
		}

		[Test]
		public void AllowsEmptyArraysIfEspeciallySet()
		{
			IMutablePicoContainer pico = GetDefaultPicoContainer();
			Type type = typeof (AnotherGenericCollectionBowl);

			pico.RegisterComponentImplementation(type, type, new IParameter[] {ComponentParameter.ARRAY_ALLOW_EMPTY});
			AnotherGenericCollectionBowl bowl = (AnotherGenericCollectionBowl) pico.GetComponentInstance(type);
			Assert.IsNotNull(bowl);
			Assert.AreEqual(0, bowl.Strings.Length);
		}

		public class TouchableObserver : ITouchable
		{
			private ITouchable[] touchables;

			public TouchableObserver(ITouchable[] touchables)
			{
				this.touchables = touchables;
			}

			public bool WasTouched
			{
				get { return touchables[0].WasTouched; }
			}

			public void Touch()
			{
				foreach (ITouchable touchable in touchables)
				{
					touchable.Touch();
				}
			}
		}

		[Test]
		public void WillOmitSelfFromCollection() 
		{
			IMutablePicoContainer pico = GetDefaultPicoContainer();
			pico.RegisterComponentImplementation(typeof(SimpleTouchable));
			pico.RegisterComponentImplementation(typeof(TouchableObserver));
			ITouchable observer = (ITouchable) pico.GetComponentInstanceOfType(typeof(TouchableObserver));
			Assert.IsNotNull(observer);
			observer.Touch();
			SimpleTouchable touchable = (SimpleTouchable) pico.GetComponentInstanceOfType(typeof(SimpleTouchable));
			Assert.IsTrue(touchable.WasTouched);
		}

		[Test]
		public void WillRemoveComponentsWithMatchingKeyFromParent() 
		{
			IMutablePicoContainer parent = new DefaultPicoContainer();
			parent.RegisterComponentImplementation("Tom", typeof(Cod));
			parent.RegisterComponentImplementation("Dick", typeof(Cod));
			parent.RegisterComponentImplementation("Harry", typeof(Cod));
			
			IMutablePicoContainer child = new DefaultPicoContainer(parent);
			child.RegisterComponentImplementation("Dick", typeof(Shark));
			child.RegisterComponentImplementation(typeof(Bowl));
			Bowl bowl = (Bowl) child.GetComponentInstance(typeof(Bowl));
			Assert.AreEqual(3, bowl.fishes.Length);
			Assert.AreEqual(2, bowl.cods.Length);
		}

		[Test]
		public void BowlWithoutTom()
		{
			IMutablePicoContainer mpc = new DefaultPicoContainer();
			mpc.RegisterComponentImplementation("Tom", typeof (Cod));
			mpc.RegisterComponentImplementation("Dick", typeof (Cod));
			mpc.RegisterComponentImplementation("Harry", typeof (Cod));
			mpc.RegisterComponentImplementation(typeof (Shark));

			IParameter[] parameters = new IParameter[]
				{
					new SampleCollectionComponentParameter(typeof (Cod), false),
					new CollectionComponentParameter(typeof (Fish), false)
				};

			mpc.RegisterComponentImplementation(typeof (CollectedBowl), typeof (CollectedBowl), parameters);

			CollectedBowl bowl = (CollectedBowl) mpc.GetComponentInstance(typeof (CollectedBowl));
			Cod tom = (Cod) mpc.GetComponentInstance("Tom");
			Assert.AreEqual(4, bowl.fishes.Length);
			Assert.AreEqual(2, bowl.cods.Length);
			Assert.IsFalse(new ArrayList(bowl.cods).Contains(tom));
		}

		/*
		[Test]
		public void DifferentCollectiveTypesAreResolved() 
		{
			IMutablePicoContainer pico = new DefaultPicoContainer();
			CollectionComponentParameter parameter = new CollectionComponentParameter(typeof(Fish), true);
			IParameter[] parameters = new IParameter[] {parameter, parameter, parameter, parameter, parameter, parameter };

			pico.RegisterComponentImplementation(typeof(DependsOnAll), typeof(DependsOnAll), parameters);
			Assert.IsNotNull(pico.GetComponentInstance(typeof(DependsOnAll)));
		}*/

		[Test]
		public void Verify()
		{
			IMutablePicoContainer pico = new DefaultPicoContainer();
			CollectionComponentParameter parameterNonEmpty = CollectionComponentParameter.ARRAY;
			pico.RegisterComponentImplementation(typeof (Shark));
			parameterNonEmpty.Verify(pico, null, typeof (Fish[]));

			try
			{
				parameterNonEmpty.Verify(pico, null, typeof (Cod[]));
				Assert.Fail("PicoIntrospectionException expected");
			}
			catch (PicoIntrospectionException e)
			{
				Assert.IsTrue(e.Message.IndexOf(typeof (Cod).Name) > -1);
			}

			CollectionComponentParameter parameterEmpty = CollectionComponentParameter.ARRAY_ALLOW_EMPTY;
			parameterEmpty.Verify(pico, null, typeof (Fish[]));
			parameterEmpty.Verify(pico, null, typeof (Cod[]));
		}

	}

	#region Test Classes

	public interface Fish
	{
	}

	public class Cod : Fish
	{
		public override String ToString()
		{
			return "Cod";
		}
	}

	public class Shark : Fish
	{
		public override String ToString()
		{
			return "Shark";
		}
	}

	public class Bowl
	{
		public Cod[] cods;
		public Fish[] fishes;

		public Bowl(Cod[] cods, Fish[] fishes)
		{
			this.cods = cods;
			this.fishes = fishes;
		}
	}

	public class CollectedBowl 
	{
		public Cod[] cods;
		public Fish[] fishes;

		public CollectedBowl(ICollection cods, ICollection fishes) 
		{
			this.cods = (Cod[]) new ArrayList(cods).ToArray(typeof(Cod));
			this.fishes = (Fish[]) new ArrayList(fishes).ToArray(typeof(Fish));
		}
	}

	/// <summary>
	/// This would be so much easier with anonymous classes!
	/// </summary>
	public class SampleCollectionComponentParameter : CollectionComponentParameter
	{
		public SampleCollectionComponentParameter(Type componentValueType, bool emptyCollection) 
			: base(componentValueType, emptyCollection)
		{
		}

		protected override bool Evaluate(IComponentAdapter componentAdapter)
		{
			return !"Tom".Equals(componentAdapter.ComponentKey);	
		}
	}

	#endregion
}
