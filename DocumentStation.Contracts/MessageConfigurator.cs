using System;

namespace DocumentStation.Contracts
{
	public enum MessageType
	{
		Unknown,
		Message,
		Command,
		Event
	}

	public class MessageConfigurator
	{
		public static Func<Type, bool> GetMessageTypeDefinition(MessageType messageType)
		{
			var postfix = GetPostfix(messageType);

			return t => t.Namespace != null && t.Namespace.StartsWith("DocumentStation.Contracts." + postfix);
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
