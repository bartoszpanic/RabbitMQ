using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Shared;

Console.Title = "RabbitMQ - czym jest kolejka? - Odbiorca dla Topic cosmos";

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Tworzenie wymiany typu topic
await channel.ExchangeDeclareAsync(exchange: "RabbitMQ.TopicExchange", type: ExchangeType.Topic);

// Deklarowanie unikalnej kolejki dla konsumenta
var queueName = await channel.QueueDeclareAsync(queue: "RabbitMQ.TopicQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

// Wiązanie kolejki z wymianą typu topic, aby odbierać tylko wiadomości typu SQL
await channel.QueueBindAsync(queue: queueName, exchange: "RabbitMQ.TopicExchange", routingKey: "query.cosmosdb");

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine("Czekam na wiadomości");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
	byte[] body = ea.Body.ToArray();
	var jobAsJson = Encoding.UTF8.GetString(body);

	var job = JsonConvert.DeserializeObject<Job>(jobAsJson);

	Console.Write(" -> Otrzymano: ");
	Console.ForegroundColor = ConsoleColor.Cyan;
	Console.WriteLine(jobAsJson);
	await Task.Delay(job.TimeOfJob * 1000);
	await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("-> Done");
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

char key = 'z';

while (key != 'q' && key != 'Q')
{
	Console.WriteLine("Naciśnij [q] aby wyjść");
	key = Console.ReadKey().KeyChar;
}