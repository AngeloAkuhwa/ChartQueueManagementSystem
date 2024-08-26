using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Messaging
{
	public interface IRabbitMqProducer
	{
		void PublishMessage(string message);
	}
}
