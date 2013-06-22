using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Ddd.Template.Contracts.Commands
{
	[Serializable]
	[DataContract]
	public abstract class Command
	{
		[HiddenInput(DisplayValue = false)]
		[Required(ErrorMessage = "Unknown aggregate id")]
		[DataMember]
		public Guid AggregateId { get; set; }

		[HiddenInput(DisplayValue = false)]
		[Required(ErrorMessage = "Unknown command id")]
		[DataMember]
		public Guid CommandId { get; set; }

		[HiddenInput(DisplayValue = false)]
		[Required(ErrorMessage = "Unknown command version")]
		[DataMember]
		public int OriginalVersion { get; set; }
	}
}
