using System.Text;

using RabbitMQ.Client;

Console.WriteLine("Press [enter] to start.");
Console.ReadLine();

var factory = new ConnectionFactory { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // fanout exchange sample

    channel.ExchangeDeclare(exchange: "sample2", type: ExchangeType.Fanout);

    var message = $"Message at {DateTime.Now}";
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "sample2",
        routingKey: "",
        basicProperties: null,
        body: body);
    Console.WriteLine("'{0}' is sent", message);
}

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();