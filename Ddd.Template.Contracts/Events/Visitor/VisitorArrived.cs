using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts.Events.Visitor
{
	[Serializable]
	[DataContract]
	public class VisitorArrived : VisitorEvent
	{
		[DataMember] public string UserAgent { get; set; }
		[DataMember] public string Platform { get; set; }
		[DataMember] public string UserHostAddress { get; set; }
		[DataMember] public string UserHostName { get; set; }
		[DataMember] public List<string> UserLanguages { get; set; }
		[DataMember] public DateTime Created { get; set; }
	}
}
