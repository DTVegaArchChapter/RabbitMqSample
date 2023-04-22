using System.Text;

using RabbitMQ.Client;

Console.WriteLine("Press [enter] to start.");
Console.ReadLine();

var factory = new ConnectionFactory { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // Direct exchange sample

    channel.QueueDeclare(queue: "sample1",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    for (var i = 0; i < 1000; i++)
    {
        var message = $"Message {i}";
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "",
            routingKey: "sample1",
            basicProperties: null,
            body: body);
        Console.WriteLine("'{0}' is sent", i);

        Thread.Sleep(25);
    }
}

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();