using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Shared;

Console.Title = "Rabbit MQ - czym jest kolejka? - Odbiorca Drugi - Jedna kolejka";


var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "RabbitMQ.MultiReciverOneQueue", type: ExchangeType.Fanout);
var queueName = await channel.QueueDeclareAsync(queue: "RabbitMQ.MultiReciverOneQueueTwoCustomer", durable: true, exclusive: false, autoDelete: false, arguments: null);

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

await channel.QueueBindAsync(queue: queueName, exchange: "RabbitMQ.MultiReciverOneQueue", routingKey: "");

Console.WriteLine("Czekam na wiadomości");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
	byte[] body = ea.Body.ToArray();
	var jobAsJson = Encoding.UTF8.GetString(body);

	var job = JsonConvert.DeserializeObject<Job>(jobAsJson);

	Console.Write(" -> Otrzymano: ");
	Console.ForegroundColor = ConsoleColor.Cyan;
	Console.WriteLine(jobAsJson);
	Thread.Sleep(job.TimeOfJob * 1000);
	channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("-> Done");
	return Task.CompletedTask;
};

channel.BasicConsumeAsync(queue: "RabbitMQ.MultiReciverOneQueueTwoCustomer", autoAck: false, consumer: consumer);

char key = 'z';

while (key != 'q' && key != 'Q')
{
	Console.WriteLine("Nacisij [q] aby wyjść");
	key = Console.ReadKey().KeyChar;
}