using Castle.Core.Logging;
using Ddd.Template.Contracts.Commands;
using NServiceBus;

namespace Ddd.Template.Server.Handlers
{
	public class MessageHandler : IHandleMessages<AddDocument>
	{
		private readonly ILogger _logger;

		public MessageHandler(ILogger logger)
		{
			_logger = logger;
		}

		public void Handle(AddDocument message)
		{
			_logger.Debug("Add document command recieved.");
		}
	}
}
