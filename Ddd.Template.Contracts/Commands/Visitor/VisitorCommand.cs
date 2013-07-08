using System;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts.Commands.Visitor
{
	[Serializable]
	[DataContract]
	public abstract class VisitorCommand : Command
	{
	}
}
