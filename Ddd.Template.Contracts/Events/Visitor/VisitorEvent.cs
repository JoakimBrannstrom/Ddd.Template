using System;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts.Events.Visitor
{
	[Serializable]
	[DataContract]
	public abstract class VisitorEvent : Event
	{
	}
}
