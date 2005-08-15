/*****************************************************************************
 * Copyright (C) PicoContainer Organization. All rights reserved.            *
 * ------------------------------------------------------------------------- *
 * The software in this package is published under the terms of the BSD      *
 * style license a copy of which has been included with this distribution in *
 * the license.txt file.                                                     *
 *                                                                           *
 * Idea by Rachel Davies, Original code by Aslak Hellesoy and Paul Hammant   *
 * C# port by Maarten Grootendorst                                           *
 *****************************************************************************/

using System;
using System.Reflection;
using System.Collections;
using PicoContainer;

namespace PicoContainer.Defaults
{
	/// <summary>
	/// Instantiates components using empty constructors and Setter Injection
	/// <remarks>
	/// <a href="http://docs.codehaus.org/display/PICO/Setter+Injection">Setter Injection</a>.
	/// For easy setting of primitive properties.  Note that this class doesn't cache instances. 
	/// If you want caching, use a <see cref="CachingComponentAdapter"/> around this one.
	/// </remarks>
	/// </summary>
	[Serializable]
	public class SetterInjectionComponentAdapter : InstantiatingComponentAdapter
	{
		[NonSerialized] private SetterInjectionGuard setterInjectionGuard;
		[NonSerialized] private IList setters;
		[NonSerialized] private Type[] setterTypes;

		public SetterInjectionComponentAdapter(Type componentImplementation) 
			: this(componentImplementation, componentImplementation)
		{
		}

		public SetterInjectionComponentAdapter(object componentKey, Type componentImplementation) 
			: this(componentKey, componentImplementation, null)
		{
		}

		public SetterInjectionComponentAdapter(Type componentImplementation, IParameter[] parameters) 
			: this(componentImplementation, componentImplementation, parameters)
		{
		}

		public SetterInjectionComponentAdapter(object componentKey,
		                                       Type componentImplementation,
		                                       IParameter[] parameters,
		                                       bool allowNonPublicClasses) : base(componentKey, componentImplementation, parameters, allowNonPublicClasses)
		{
		}

		public SetterInjectionComponentAdapter(object componentKey,
		                                       Type componentImplementation,
		                                       IParameter[] parameters) : base(componentKey, componentImplementation, parameters, false)
		{
		}

		protected override ConstructorInfo GetGreediestSatisfiableConstructor(IPicoContainer container)
		{
			return GetConstructor();
		}

		private ConstructorInfo GetConstructor()
		{
			return ComponentImplementation.GetConstructor(Type.EmptyTypes);
		}

		private IParameter[] GetMatchingParameterListForSetters(IPicoContainer container)
		{
			if (setters == null)
			{
				InitializeSetterAndTypeLists();
			}

			IParameter[] matchingParameterList = new IParameter[setters.Count];
			ArrayList nonMatchingParameterPositions = new ArrayList(); // was Set
			IParameter[] currentParameters = parameters != null ? parameters : CreateDefaultParameters(setterTypes);
			for (int i = 0; i < currentParameters.Length; i++)
			{
				IParameter parameter = currentParameters[i];
				bool failedDependency = true;
				for (int j = 0; j < setterTypes.Length; j++)
				{
					if (matchingParameterList[j] == null && parameter.IsResolvable(container, this, setterTypes[j]))
					{
						matchingParameterList[j] = parameter;
						failedDependency = false;
						break;
					}
				}
				if (failedDependency)
				{
					nonMatchingParameterPositions.Add(i);
				}
			}

			ArrayList unsatisfiableDependencyTypes = new ArrayList();
			for (int i = 0; i < matchingParameterList.Length; i++)
			{
				if (matchingParameterList[i] == null)
				{
					unsatisfiableDependencyTypes.Add(setterTypes[i]);
				}
			}
			if (unsatisfiableDependencyTypes.Count > 0)
			{
				throw new UnsatisfiableDependenciesException(this, unsatisfiableDependencyTypes);
			}
			else if (nonMatchingParameterPositions.Count > 0)
			{
				throw new PicoInitializationException("Following parameters do not match any of the setters for "
					+ ComponentImplementation + ": " + nonMatchingParameterPositions.ToString());
			}
			return matchingParameterList;
		}

		public override Object GetComponentInstance(IPicoContainer container)
		{
			if (setterInjectionGuard == null)
			{
				setterInjectionGuard = new SetterInjectionGuard(container, this);
			}
			return setterInjectionGuard.Observe(ComponentImplementation);
		}

		public override void Verify(IPicoContainer container)
		{
			if (verifyingGuard == null)
			{
				verifyingGuard = new VerifyingGuard(this, container);
			}
			verifyingGuard.Observe(ComponentImplementation);
		}

		private void InitializeSetterAndTypeLists()
		{
			setters = new ArrayList();
			ArrayList typeList = new ArrayList();

			PropertyInfo[] properties = ComponentImplementation.GetProperties();
			foreach (PropertyInfo property in properties)
			{
				MethodInfo method = property.GetSetMethod();
				if (method != null)
				{
					setters.Add(method);
					typeList.Add(property.PropertyType);
				}
			}

			setterTypes = (Type[]) typeList.ToArray(typeof (Type));
		}

		[Serializable]
		protected class SetterInjectionGuard : ThreadStaticCyclicDependencyGuard
		{
			private IPicoContainer guardedContainer;
			private SetterInjectionComponentAdapter sica;
			
			public SetterInjectionGuard(IPicoContainer guardedContainer, SetterInjectionComponentAdapter sica)
			{
				this.guardedContainer = guardedContainer;
				this.sica = sica;
			}

			public override Object Run()
			{
				ConstructorInfo constructorInfo = sica.GetConstructor();
				IParameter[] matchingParameters = sica.GetMatchingParameterListForSetters(guardedContainer);
				try
				{
					object componentInstance = constructorInfo.Invoke(new object[] {}); // removed newInstance call
					for (int i = 0; i < sica.setters.Count; i++)
					{
						MethodInfo setter = (MethodInfo) sica.setters[i];
						setter.Invoke(componentInstance, new Object[] {matchingParameters[i].ResolveInstance(guardedContainer, sica, sica.setterTypes[i])});
					}
					return componentInstance;
				}
				catch (TargetInvocationException e)
				{
					throw new PicoInvocationTargetInitializationException(e.GetBaseException());
				}
				catch(NullReferenceException e)
				{
					throw new PicoInvocationTargetInitializationException(e.GetBaseException());
				}
			}
		}

		[Serializable]
		class VerifyingGuard : InstantiatingComponentAdapter.DefaultVerifyingGuard
		{
			public VerifyingGuard(InstantiatingComponentAdapter ica, IPicoContainer guardedContainer) 
				: base(ica, guardedContainer)
			{
			}

			public override Object Run()
			{
				SetterInjectionComponentAdapter sica = (SetterInjectionComponentAdapter)ica;

				IParameter[] currentParameters = sica.GetMatchingParameterListForSetters(guardedContainer);
				for (int i = 0; i < currentParameters.Length; i++)
				{
					currentParameters[i].Verify(guardedContainer, sica, sica.setterTypes[i]);
				}
				return null;
			}
		}
	}
}