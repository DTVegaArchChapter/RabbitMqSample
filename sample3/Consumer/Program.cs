using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var sleepMs = 0;
if (args.Length > 0)
{
    _ = int.TryParse(args[0], out sleepMs);
}

var factory = new ConnectionFactory { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // ack / nack sample

    channel.QueueDeclare(queue: "sample3",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received '{0}'", message);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            // channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);

            Thread.Sleep(sleepMs);
        };
    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    channel.BasicConsume(queue: "sample3", autoAck: false, consumer: consumer);

    Console.WriteLine("Press [enter] to exit.");
    Console.ReadLine();
}