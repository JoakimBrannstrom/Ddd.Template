using System;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts.Commands.Visitor
{
	[Serializable]
	[DataContract]
	public sealed class AddVisitorLogin : VisitorCommand
	{
		[DataMember] public Guid UserId { get; set; }
	}
}
