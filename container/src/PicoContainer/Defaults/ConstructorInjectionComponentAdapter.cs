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
using System.Collections;
using System.Reflection;
using System.Text;
using PicoContainer;
using PicoContainer.Utils;

namespace PicoContainer.Defaults
{
	/// <summary>
	/// Instantiates components using Constructor Injection.
	/// <remarks>
	/// Note that this class doesn't cache instances. If you want caching,
	/// use a <see cref="CachingComponentAdapter"/> around this one.
	/// </remarks>
	/// </summary>
	[Serializable]
	public class ConstructorInjectionComponentAdapter : InstantiatingComponentAdapter
	{
		private IList sortedMatchingConstructors;
		private ConstructorInjectionGuard constructorInjectionGuard;
		private IComponentMonitor componentMonitor = new NullComponentMonitor();

		public IComponentMonitor ComponentMonitor
		{
			get { return componentMonitor; }
		}

		public ConstructorInjectionComponentAdapter(object componentKey, Type componentImplementation, IParameter[] parameters, bool allowNonPublicClasses, IComponentMonitor componentMonitor)
			: base(componentKey, componentImplementation, parameters, allowNonPublicClasses)
		{
			this.componentMonitor = componentMonitor;
		}

		public ConstructorInjectionComponentAdapter(Object componentKey, Type componentImplementation, IParameter[] parameters)
			: this(componentKey, componentImplementation, parameters, false, new NullComponentMonitor())
		{
		}

		/// <summary>
		/// Use default parameters.
		/// </summary>
		/// <param name="componentKey"></param>
		/// <param name="componentImplementation"></param>
		public ConstructorInjectionComponentAdapter(Object componentKey, Type componentImplementation)
			: this(componentKey, componentImplementation, null)
		{
		}

		public override object GetComponentInstance(IPicoContainer container)
		{
			if(constructorInjectionGuard == null)
			{
				constructorInjectionGuard = new ConstructorInjectionGuard(this, container);
			}
			return constructorInjectionGuard.Observe(ComponentImplementation);
		}

		protected override ConstructorInfo GetGreediestSatisfiableConstructor(IPicoContainer container)
		{
			ConstructorInfo greediestConstructor = null;
			IList conflicts = new ArrayList();
			IList unsatisfiableDependencyTypes = new ArrayList();
			if (sortedMatchingConstructors == null)
			{
				sortedMatchingConstructors = GetSortedMatchingConstructors();
			}
			int lastSatisfiableConstructorSize = -1;
			for (int i = 0; i < sortedMatchingConstructors.Count; i++)
			{
				bool failedDependency = false;
				ConstructorInfo constructor = (ConstructorInfo) sortedMatchingConstructors[i];
				Type[] parameterTypes = TypeUtils.GetParameterTypes(constructor.GetParameters());
				IParameter[] currentParameters = parameters != null ? parameters : CreateDefaultParameters(parameterTypes);

				// remember: all constructors with less arguments than the given parameters are filtered out already
				for (int j = 0; j < currentParameters.Length; j++)
				{
					// check wether this constructor is statisfiable
					if (currentParameters[j].IsResolvable(container, this, parameterTypes[j]))
					{
						continue;
					}

					foreach (Type type in parameterTypes)
					{
						unsatisfiableDependencyTypes.Add(type);
					}
					failedDependency = true;
					break;
				}

				if (greediestConstructor != null && parameterTypes.Length != lastSatisfiableConstructorSize)
				{
					if (conflicts.Count == 0)
					{
						// we found our match [aka. greedy and satisfied]
						return greediestConstructor;
					}
					else
					{
						// fits although not greedy
						conflicts.Add(constructor);
					}
				}
				else if (!failedDependency && lastSatisfiableConstructorSize == parameterTypes.Length)
				{
					// satisfied and same size as previous one?
					conflicts.Add(constructor);
					conflicts.Add(greediestConstructor);
				}
				else if (!failedDependency)
				{
					greediestConstructor = constructor;
					lastSatisfiableConstructorSize = parameterTypes.Length;
				}
			}
			if (conflicts.Count != 0)
			{
				throw new TooManySatisfiableConstructorsException(ComponentImplementation, conflicts);
			}
			else if (greediestConstructor == null && unsatisfiableDependencyTypes.Count != 0)
			{
				throw new UnsatisfiableDependenciesException(this, unsatisfiableDependencyTypes);
			}
			else if (greediestConstructor == null)
			{
				// be nice to the user, show all constructors that were filtered out 
				StringBuilder ctorDetails = new StringBuilder();
				ConstructorInfo[] constructors = GetAllConstructorInfo();

				foreach (ConstructorInfo constructor in constructors)
				{
					ctorDetails.Append(TypeUtils.ConstructorAsString(constructor));
				}
				throw new PicoInitializationException("Either do the specified parameters not match any of the following constructors: " 
					+ ctorDetails
					+ " or the constructors were not accessible for '" + ComponentImplementation + "'");
			}
			return greediestConstructor;
		}


		protected object[] GetConstructorArguments(IPicoContainer container, ConstructorInfo ctor)
		{
			Type[] parameterTypes = TypeUtils.GetParameterTypes(ctor.GetParameters());
			Object[] result = new Object[parameterTypes.Length];
			IParameter[] currentParameters = parameters != null ? parameters : CreateDefaultParameters(parameterTypes);

			for (int i = 0; i < currentParameters.Length; i++)
			{
				result[i] = currentParameters[i].ResolveInstance(container, this, parameterTypes[i]);
			}
			return result;
		}

		/// <summary>
		/// Gets all valid constructors. Non Public constructors are returned if the allowNonPublicClasses flag
		/// was set accordingly.
		/// </summary>
		/// <returns></returns>
		private ConstructorInfo[] GetAllConstructorInfo()
		{
			if (allowNonPublicClasses)
			{
				return ComponentImplementation.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}

			return ComponentImplementation.GetConstructors();
		}

		private IList GetSortedMatchingConstructors()
		{
			ArrayList matchingConstructors = new ArrayList();
			ConstructorInfo[] allConstructors = GetAllConstructorInfo();

			// filter out all constructors that will definately not match 
			for (int i = 0; i < allConstructors.Length; i++)
			{
				ConstructorInfo constructor = allConstructors[i];
				Type[] parameterTypes = Utils.TypeUtils.GetParameterTypes(constructor);

				if (parameters == null || parameterTypes.Length == parameters.Length)
				{
					matchingConstructors.Add(constructor);
				}
			}
			// optimize list of constructors moving the longest at the beginning
			if (parameters == null)
			{
				matchingConstructors.Sort(new ContructorComparator());
			}
			return matchingConstructors;
		}

		class ContructorComparator : IComparer
		{
			public int Compare(Object arg0, Object arg1)
			{
				Type[] parameterTypes0 = Utils.TypeUtils.GetParameterTypes((ConstructorInfo) arg0);
				Type[] parameterTypes1 = Utils.TypeUtils.GetParameterTypes((ConstructorInfo) arg1);

				return parameterTypes1.Length - parameterTypes0.Length;
			}
		}

		/// <summary>
		/// Guard description...
		/// </summary>
		[Serializable]
		protected class ConstructorInjectionGuard : ThreadStaticCyclicDependencyGuard 
		{
			protected ConstructorInjectionComponentAdapter cica;
			protected IPicoContainer guardedContainer;

			public ConstructorInjectionGuard(ConstructorInjectionComponentAdapter cica, IPicoContainer container)
			{
				this.cica = cica;
				this.guardedContainer = container;
			}

			public override Object Run() 
			{
				ConstructorInfo constructor;
				try 
				{
					constructor = cica.GetGreediestSatisfiableConstructor(guardedContainer);
				} 
				catch (AmbiguousComponentResolutionException e) 
				{
					e.Component = cica.ComponentImplementation;
					throw e;
				}
				try 
				{
					Object[] parameters = cica.GetConstructorArguments(guardedContainer, constructor);
					cica.ComponentMonitor.Instantiating(constructor);
					long startTime = DateTime.Now.Millisecond;
					Object inst = constructor.Invoke(parameters);
					cica.componentMonitor.Instantiated(constructor, startTime, DateTime.Now.Millisecond - startTime);
					return inst;
				} 
				catch (Exception e)
				{
					cica.componentMonitor.InstantiationFailed(constructor, e);
					if(e is TargetInvocationException)
					{
						throw new PicoInvocationTargetInitializationException(e);	
					}
					else if (e is SystemException || e is ApplicationException) 
					{
						throw e.GetBaseException();
					} 
					else
					{
						throw new PicoInitializationException(e);
					}
				}
			}
		}
	}
}