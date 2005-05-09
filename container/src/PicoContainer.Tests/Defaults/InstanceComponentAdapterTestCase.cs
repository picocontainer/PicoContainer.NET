using System;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.Tck;
using PicoContainer.TestModel;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class InstanceComponentAdapterTestCase : AbstractComponentAdapterTestCase
	{
		public void testIComponentAdapterReturnsSame() 
		{
			ITouchable touchable = new SimpleTouchable();
			IComponentAdapter IComponentAdapter = new InstanceComponentAdapter(typeof(ITouchable), touchable);
			Assert.AreSame(touchable, IComponentAdapter.GetComponentInstance(null));
		}

		protected override Type GetComponentAdapterType() 
		{
			return typeof(InstanceComponentAdapter);
		}
    
		protected override int GetComponentAdapterNature() 
		{
			return base.GetComponentAdapterNature() & ~(RESOLVING | VERIFYING | INSTANTIATING );
		}
    
		protected override IComponentAdapter prepDEF_verifyWithoutDependencyWorks(IMutablePicoContainer picoContainer) 
		{
			return new InstanceComponentAdapter("foo", "bar");
		}
    
		protected override IComponentAdapter prepDEF_verifyDoesNotInstantiate(
			IMutablePicoContainer picoContainer) 
		{
			return new InstanceComponentAdapter("Key", 4711);
		}
    
		protected override IComponentAdapter prepSER_isSerializable(IMutablePicoContainer picoContainer) 
		{
			return new InstanceComponentAdapter("Key", 4711);
		}


		
	}
}
