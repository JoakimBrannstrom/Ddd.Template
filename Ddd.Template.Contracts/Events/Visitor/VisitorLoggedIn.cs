using System;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts.Events.Visitor
{
	[Serializable]
	[DataContract]
	public class VisitorLoggedIn : VisitorEvent
	{
		[DataMember] public Guid UserId { get; set; }
		[DataMember] public string ClaimedIdentifier { get; set; }
	}
}
