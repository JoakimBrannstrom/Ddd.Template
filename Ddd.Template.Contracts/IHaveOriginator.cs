using System;
using System.Runtime.Serialization;

namespace Ddd.Template.Contracts
{
	public interface IHaveOriginator
	{
		[DataMember]
		Guid OriginatorId { get; set; }
	}
}
