using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMqProductAPI.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private IModel Channel { get; }

        public RabbitMQProducer(IModel channel)
        {
            Channel = channel;
        }

        public void SendProductMessage<T>(T message)
        {
            //Serialize the message
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            //declare the queue after mentioning name and a few property related to that
            Channel.QueueDeclare("product", exclusive: false);

            //put the data on to the product queue
            Channel.BasicPublish(exchange: "", routingKey: "product", body: body);
        }
    }
}
