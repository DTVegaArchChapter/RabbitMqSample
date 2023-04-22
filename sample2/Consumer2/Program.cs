using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "sample2", type: ExchangeType.Fanout);

    channel.QueueDeclare(queue: "sample2_queue2",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);
    channel.QueueBind(queue: "sample2_queue2",
        exchange: "sample2",
        routingKey: "");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received '{0}'", message);
        };
    channel.BasicConsume(queue: "sample2_queue2",
        autoAck: true,
        consumer: consumer);

    Console.WriteLine("Press [enter] to exit.");
    Console.ReadLine();
}