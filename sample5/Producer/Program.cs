using System.Text;

using RabbitMQ.Client;

Console.WriteLine("Press [enter] to start.");
Console.ReadLine();

var factory = new ConnectionFactory { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
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

    for (var i = 0; i < 1000; i++)
    {
        var message = $"Message {i}";
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "",
            routingKey: "sample5",
            basicProperties: null,
            body: body);
        Console.WriteLine("'{0}' is sent", i);

        Thread.Sleep(25);
    }
}

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();