using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json")
     .AddEnvironmentVariables()
     .Build();

//Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
var factory = new ConnectionFactory
{
    HostName = configuration["RabbitMQ:Hostname"],
    UserName = configuration["RabbitMQ:Username"],
    Password = configuration["RabbitMQ:Password"]
};

//Create the RabbitMQ connection using connection factory details as i mentioned above
var connection = factory.CreateConnection();

//Here we create channel with session and model
using var channel = connection.CreateModel();

//declare the queue after mentioning name and a few property related to that
channel.QueueDeclare("product", exclusive: false);

//Set Event object which listen message from chanel which is sent by producer
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Product message received: {message}");
};

//read the message
channel.BasicConsume(queue: "product", autoAck: true, consumer: consumer);

//avoid docker container to close
await Task.Run(() => Thread.Sleep(Timeout.Infinite));