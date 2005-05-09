using System;
using NUnit.Framework;

namespace PicoContainer.TestModel
{
	/// <summary>
	/// Summary description for DependsOnTwoComponents.
	/// </summary>
	public class DependsOnTwoComponents
	{
		public DependsOnTwoComponents(ITouchable Touchable, DependsOnTouchable fred)
		{
			//    Assert.IsNotNull("Touchable cannot be passed in as null", Touchable);
//      Assert.IsNotNull("DependsOnTouchable cannot be passed in as null", fred);
		}
	}
}