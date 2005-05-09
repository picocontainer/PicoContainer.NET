using System;
using NUnit.Framework;
using PicoContainer;
using PicoContainer.Defaults;
using PicoContainer.Tck;

namespace PicoContainer.Defaults
{
	[TestFixture]
	public class DefaultLazyInstantiationTestCase : AbstractLazyInstantiationTestCase
	{
		protected override IMutablePicoContainer createPicoContainer()
		{
			return new DefaultPicoContainer();
		}
	}
}