using System;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Commands;

namespace Ddd.Template.Domain.CommandHandlers
{
	public class CommandHandlerBase<T> where T : AggregateRoot, new()
	{
		protected readonly IRepository<T> Repository;

		protected CommandHandlerBase(IRepository<T> repository)
		{
			Repository = repository;
		}

		protected void HandleCommandOnExistingAggregate(Command command, Action<T> action)
		{
			var aggregate = Repository.GetById(command.AggregateId);
			action(aggregate);

			var originatorId = Guid.Empty;
			var originatorCommand = command as IHaveOriginator;
			if (originatorCommand != null)
				originatorId = originatorCommand.OriginatorId;

			Repository.Save(command.CommandId, aggregate, command.OriginalVersion, originatorId);
		}
	}
}
