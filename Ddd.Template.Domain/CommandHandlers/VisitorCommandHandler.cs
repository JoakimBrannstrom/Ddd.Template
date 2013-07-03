using System;
using Ddd.Template.Contracts.Commands.Visitor;
using Ddd.Template.Domain.Aggregates;
using NServiceBus;

namespace Ddd.Template.Domain.CommandHandlers
{
	public class VisitorCommandHandler : CommandHandlerBase<Visitor>,
										IHandleMessages<AddVisitor>,
										IHandleMessages<AddVisitorLogin>
	{
		public VisitorCommandHandler(IRepository<Visitor> repository)
			: base(repository)
		{
		}

		public void Handle(AddVisitor command)
		{
			Console.WriteLine("{0} - {1} command received.", DateTime.Now, command.GetType().Name);

			var visitor = new Visitor(command.AggregateId, command.Created, command.Platform, command.UserAgent,
									command.UserHostAddress, command.UserHostName, command.UserLanguages);

			Repository.Save(command.CommandId, visitor, 0, Guid.Empty);
		}

		public void Handle(AddVisitorLogin command)
		{
			HandleCommandOnExistingAggregate(command, item => item.Login(command.UserId));
		}
	}
}
