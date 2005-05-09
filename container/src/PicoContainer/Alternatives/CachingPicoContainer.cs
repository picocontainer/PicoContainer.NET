using System;
using PicoContainer;
using PicoContainer.Defaults;

namespace PicoContainer.Alternatives
{
	[Serializable]
	public class CachingPicoContainer : AbstractDelegatingMutablePicoContainer
	{
		private ILifecycleManager lifecycleManager;
		private CachingComponentAdapterFactory caf;

		/// <summary>
		/// Creates a new container with a parent container.
		/// </summary>
		/// <param name="caf"></param>
		/// <param name="parent"></param>
		/// <param name="lifecycleManager"></param>
		public CachingPicoContainer(CachingComponentAdapterFactory caf, IPicoContainer parent, ILifecycleManager lifecycleManager) : base(new DefaultPicoContainer(caf, parent, lifecycleManager))
		{
			this.lifecycleManager = lifecycleManager;
			this.caf = caf;
		}

		/// <summary>
		/// Creates a new container with a parent container.
		/// </summary>
		/// <param name="caf"></param>
		/// <param name="parent"></param>
		public CachingPicoContainer(CachingComponentAdapterFactory caf, IPicoContainer parent) : this(caf, parent, new DefaultLifecycleManager())
		{
		}

		public CachingPicoContainer(IComponentAdapterFactory caf, IPicoContainer parent) : this(new CachingComponentAdapterFactory(caf), parent)
		{
		}

		/// <summary>
		/// Creates a new container with a parent container.
		/// </summary>
		/// <param name="parent"></param>
		public CachingPicoContainer(IPicoContainer parent) : this(new DefaultComponentAdapterFactory(), parent)
		{
		}

		/// <summary>
		/// Creates a new container with a parent container.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="lifecycleManager"></param>
		public CachingPicoContainer(IPicoContainer parent, ILifecycleManager lifecycleManager) : this(new CachingComponentAdapterFactory(new DefaultComponentAdapterFactory()), parent, lifecycleManager)
		{
		}

		/// <summary>
		/// Creates a new container with a parent container.
		/// </summary>
		/// <param name="caf"></param>
		public CachingPicoContainer(IComponentAdapterFactory caf) : this(caf, null)
		{
		}

		/// <summary>
		/// Creates a new container with no parent container.
		/// </summary>
		public CachingPicoContainer() : this((IPicoContainer) null)
		{
		}

		public override IMutablePicoContainer MakeChildContainer()
		{
			ImplementationHidingCachingPicoContainer pc = new ImplementationHidingCachingPicoContainer(caf, this, lifecycleManager);
			DelegateContainer.AddChildContainer(pc);
			return pc;
		}

	}
}