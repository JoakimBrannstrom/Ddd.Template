using Castle.Core.Logging;
using DocumentStation.Contracts.Commands;
using NServiceBus;

namespace DocumentStation.Server.Handlers
{
	class MessageHandler : IHandleMessages<AddDocument>
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
