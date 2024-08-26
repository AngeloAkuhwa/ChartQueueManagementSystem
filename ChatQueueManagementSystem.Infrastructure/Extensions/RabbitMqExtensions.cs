using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ChatQueueManagementSystem.Infrastructure.Settings;
using ChatQueueManagementSystem.Application.Common.Interfaces.Messaging;
using ChatQueueManagementSystem.Infrastructure.Messaging;
using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using System;

namespace ChatQueueManagementSystem.Infrastructure.Extensions
{
	public static class RabbitMqExtensions
	{
		public static async Task<IServiceCollection> AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
		{
			var queueName = await GetAnyAvailableQueueNameAsync(services);

			services.AddSingleton<IConnectionFactory>(sp =>
			{
				var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
				return new ConnectionFactory
				{
					HostName = rabbitMqSettings.HostName,
					UserName = rabbitMqSettings.UserName,
					Password = rabbitMqSettings.Password,
					Port = rabbitMqSettings.Port,
					VirtualHost = rabbitMqSettings.VirtualHost
				};
			});

			services.AddSingleton<IConnection>(sp =>
			{
				var factory = sp.GetRequiredService<IConnectionFactory>();
				return factory.CreateConnection();
			});

			services.AddSingleton<IModel>(sp =>
			{
				var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
				var connection = sp.GetRequiredService<IConnection>();
				var channel = connection.CreateModel();
				channel.ExchangeDeclare(rabbitMqSettings.ExchangeName, ExchangeType.Direct, true);
				
				channel.QueueDeclare(queue: rabbitMqSettings.DeadLetterQueueName,
					durable: true,
					exclusive: false,
					autoDelete: false,
					arguments: new Dictionary<string, object>
					{
						{ "x-message-ttl", rabbitMqSettings.RetryDelayInMilliseconds },
						{ "x-dead-letter-exchange", rabbitMqSettings.ExchangeName },
						{ "x-dead-letter-routing-key", queueName }
					});


				//primary queue declaration
				channel.QueueDeclare(queue: queueName, durable: true,
					exclusive: false,
					autoDelete: false,
					arguments: new Dictionary<string, object>
					{
						{ "x-dead-letter-exchange", rabbitMqSettings.DeadLetterExchangeName },
						{ "x-dead-letter-routing-key", rabbitMqSettings.DeadLetterRoutingKey }
					});

				channel.QueueBind(queue: queueName, rabbitMqSettings.ExchangeName, rabbitMqSettings.RoutingKey);
				return channel;
			});

			services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();
			services.AddHostedService<RabbitMqConsumer>();

			return services;
		}

		private static async Task<string> GetAnyAvailableQueueNameAsync(IServiceCollection services)
		{
			var serviceProvider = services.BuildServiceProvider();
			var queueRepository = serviceProvider.GetRequiredService<IQueueRepository>();

			var queue = await queueRepository.GetQueueByTypeAsync(false);

			return queue.QueueName;
		}
	}
}