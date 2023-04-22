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
    // dead letter exchange sample

    channel.ExchangeDeclare(exchange: "sample5-dlx", type: ExchangeType.Direct);
    channel.QueueDeclare(
        queue: "sample5-dlx",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);
    channel.QueueBind("sample5-dlx", "sample5-dlx", "sample5-dlx");

    channel.QueueDeclare(
        queue: "sample5",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: new Dictionary<string, object>
                       {
                           { "x-dead-letter-exchange", "sample5-dlx" },
                           { "x-dead-letter-routing-key", "sample5-dlx" }
                       });

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received '{0}'", message);
            
            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);

            Thread.Sleep(sleepMs);
        };
    channel.BasicConsume(queue: "sample5", autoAck: false, consumer: consumer);

    Console.WriteLine("Press [enter] to exit.");
    Console.ReadLine();
}