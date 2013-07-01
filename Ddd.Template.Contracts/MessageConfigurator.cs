using System;

namespace Ddd.Template.Contracts
{
	public enum MessageType
	{
		Unknown,
		Message,
		Command,
		Event
	}

	public static class MessageConfigurator
	{
		public static Func<Type, bool> GetMessageTypeDefinition(MessageType messageType)
		{
			var postfix = GetPostfix(messageType);

			return t => t.Namespace != null && t.Namespace.StartsWith("Ddd.Template.Contracts." + postfix);
		}

		private static string GetPostfix(MessageType messageType)
		{
			switch (messageType)
			{
				case MessageType.Command:
					return "Commands";
				case MessageType.Event:
					return "Events";
				default:
					throw new ArgumentOutOfRangeException("messageType");
			}
		}
	}
}
