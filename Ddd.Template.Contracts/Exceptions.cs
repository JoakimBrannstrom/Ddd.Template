using System;

namespace Ddd.Template.Contracts
{
	[Serializable]
	public class ConcurrencyException : Exception
	{
		public ConcurrencyException(string message)
			: base(message)
		{
		}
	}

	[Serializable]
	public class AggregateNotFoundException : Exception
	{
		public AggregateNotFoundException(string message)
			: base(message)
		{
		}
	}
}
