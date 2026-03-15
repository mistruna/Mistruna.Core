using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using Mistruna.Core.Contracts.Base.Infrastructure;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Attributes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Implementations;

public class RabbitManager(IPooledObjectPolicy<IModel> objectPolicy) : IRabbitManager
{
    private readonly DefaultObjectPool<IModel> _objectPool = new(objectPolicy, Environment.ProcessorCount * 2);

    public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class
    {
        if (message == null)
        {
            return;
        }

        var model = _objectPool.Get();
        try
        {
            model.ExchangeDeclare(exchangeName, exchangeType, true, false, null);
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var basicProperties = model.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Type = message.GetType().Name;
            model.BasicPublish(exchangeName, routeKey, basicProperties, (ReadOnlyMemory<byte>)bytes);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            _objectPool.Return(model);
        }
    }

    public T Subscribe<T>(string queueName, Guid id) where T : IEntity
    {
        var respQueue = new BlockingCollection<T>();
        var channel = _objectPool.Get();
        channel.QueueDeclare(queueName, true, false, false, null);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (EventHandler<BasicDeliverEventArgs>)((ch, ea) =>
        {
            var obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body.ToArray()));
            channel.BasicAck(ea.DeliveryTag, false);
            if (obj.Id != id)
            {
                return;
            }

            respQueue.Add(obj);
            respQueue.CompleteAdding();
        });
        channel.BasicConsume(queueName, false, consumer);
        return respQueue.Take();
    }

    public void Send<T>(T message) where T : class
    {
        if (message == null)
        {
            return;
        }

        var model = _objectPool.Get();
        if (typeof(T).GetCustomAttributes(typeof(RabbitQueryAttribute)).FirstOrDefault()
            is not RabbitQueryAttribute rabbitQueryAttribute)
        {
            throw new Exception("The RabbitQueryAttribute attribute is not exist");
        }

        try
        {
            model.ExchangeDeclare(
                rabbitQueryAttribute.ExchangeName,
                rabbitQueryAttribute.ExchangeType,
                true,
                false,
                null);
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var basicProperties = model.CreateBasicProperties();
            basicProperties.Persistent = true;
            model.BasicPublish(
                rabbitQueryAttribute.ExchangeName,
                rabbitQueryAttribute.RouteKey,
                basicProperties,
                (ReadOnlyMemory<byte>)bytes);
        }
        finally
        {
            _objectPool.Return(model);
        }
    }

    public void Consume<T, TE>(Func<T, TE> lambda)
    {
        if (typeof(T).GetCustomAttributes(typeof(RabbitQueryAttribute)).FirstOrDefault()
            is not RabbitQueryAttribute)
        {
            throw new Exception("The RabbitQueryAttribute attribute is not exist");
        }

        var channel = _objectPool.Get();
        channel.QueueDeclare(
            Assembly.GetExecutingAssembly().FullName + nameof(T),
            true,
            false,
            false,
            null);
        new EventingBasicConsumer(channel).Received += (EventHandler<BasicDeliverEventArgs>)(async (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            try
            {
                var message = JsonConvert.DeserializeObject<T>(content);
                var res = lambda(message);
                message = default(T);
                res = default(TE);
            }
            finally
            {
                channel.BasicAck(ea.DeliveryTag, false);
            }

            content = null;
        });
    }
}
