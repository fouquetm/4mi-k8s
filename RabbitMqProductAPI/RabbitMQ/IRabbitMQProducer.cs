using RabbitMQ.Client;

namespace RabbitMqProductAPI.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message);
    }
}
