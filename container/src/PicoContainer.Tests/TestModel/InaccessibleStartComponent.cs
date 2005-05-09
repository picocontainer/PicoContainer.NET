using System;
using System.Collections;
using NUnit.Framework;

namespace PicoContainer.TestModel
{
	/// <summary>
	/// Summary description for InaccessibleStartComponent.
	/// </summary>
	public class InaccessibleStartComponent
	{
		private IList messages;

		public InaccessibleStartComponent(IList messages)
		{
			this.messages = messages;
		}

		private void start()
		{
			messages.Add("Started");
		}
	}
}