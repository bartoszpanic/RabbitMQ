using System.Text;
using RabbitMQ.Client;

Console.Title = "RabbitMQ - czym jest kolejka?";

var factory = new ConnectionFactory() {HostName = "localhost"};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


await channel.QueueDeclareAsync(queue: "RabbitMQ", durable: true, exclusive: false, autoDelete: false, arguments: null);

while (true)
{
	Console.WriteLine("Napisz wiadomość do wysłania");
	Console.WriteLine("Napisz exit aby wyjść");

	string message = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(message) || message == "exit")
	{
		break;
	}

	var body  = Encoding.UTF8.GetBytes(message);

	await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "RabbitMQ", body : body);


	Console.WriteLine("");
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("\tSend {0}", message);
	Console.ForegroundColor = ConsoleColor.Gray;
	Console.WriteLine("");
}
