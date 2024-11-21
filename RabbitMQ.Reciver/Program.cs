using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.Title = "Rabbit MQ - czym jest kolejka? - Odbiorca";


var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


await channel.QueueDeclareAsync(queue: "RabbitMQ", durable: true, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine("Czekam na wiadomości");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
	var body = Encoding.UTF8.GetString(ea.Body.ToArray());
	Console.Write(" -> Otrzymano: ");
	Console.ForegroundColor = ConsoleColor.Cyan;
	Console.WriteLine(body);
	Console.ForegroundColor = ConsoleColor.Gray;
	return Task.CompletedTask;
};

channel.BasicConsumeAsync(queue: "RabbitMQ", autoAck: true, consumer: consumer);

char key = 'z';

while (key != 'q' && key != 'Q')
{
	Console.WriteLine("Nacisij [q] aby wyjść");
	key = Console.ReadKey().KeyChar;
}