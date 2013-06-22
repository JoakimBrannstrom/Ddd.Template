using System;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts.Events
{
	[Serializable]
	[DataContract]
	public abstract class Event
	{
		[DataMember]
		public Guid AggregateId { get; set; }
		[DataMember]
		public int Version { get; set; }
		[DataMember]
		public DateTime UtcTimestamp { get; set; }
	}
}
