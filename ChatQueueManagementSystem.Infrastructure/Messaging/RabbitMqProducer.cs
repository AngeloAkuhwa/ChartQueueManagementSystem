using System.Text;
using ChatQueueManagementSystem.Application.Common.Interfaces.Messaging;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ChatQueueManagementSystem.Infrastructure.Messaging
{
	public class RabbitMqProducer : IRabbitMqProducer, IDisposable
	{
		private readonly IModel _channel;
		private readonly RabbitMqSettings _rabbitMqSettings;

		public RabbitMqProducer(IModel channel, IOptions<RabbitMqSettings> rabbitMqSettings)
		{
			_channel = channel;
			_rabbitMqSettings = rabbitMqSettings.Value;
		}

		public void PublishMessage(string message)
		{
			var body = Encoding.UTF8.GetBytes(message);
			_channel.BasicPublish(exchange: _rabbitMqSettings.ExchangeName, routingKey: _rabbitMqSettings.RoutingKey, basicProperties: null, body: body);
		}

		public void Dispose()
		{
			_channel?.Close();
		}
	}
}