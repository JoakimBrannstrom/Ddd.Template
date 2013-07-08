using System;

namespace Ddd.Template.Contracts
{
	[Serializable]
	public sealed class ConcurrencyException : Exception
	{
		public ConcurrencyException(string message)
			: base(message)
		{
		}
	}

	[Serializable]
	public sealed class AggregateNotFoundException : Exception
	{
		public AggregateNotFoundException(string message)
			: base(message)
		{
		}
	}
}
