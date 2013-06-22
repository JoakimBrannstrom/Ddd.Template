using System;

namespace Ddd.Template.Contracts
{
	public class ConcurrencyException : Exception
	{
		public ConcurrencyException(string message)
			: base(message)
		{
		}
	}

	public class AggregateNotFoundException : Exception
	{
		public AggregateNotFoundException(string message)
			: base(message)
		{
		}
	}
}
