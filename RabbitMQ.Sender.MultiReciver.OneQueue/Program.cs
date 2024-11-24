using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Shared;
using System.Text;

Console.Title = "RabbitMQ - czym jest kolejka?";

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


await channel.ExchangeDeclareAsync(exchange: "RabbitMQ.MultiReciverOneQueue", type: ExchangeType.Fanout);
await channel.QueueDeclareAsync(queue: "RabbitMQ.MultiReciverOneQueueTwoCustomer", durable: true, exclusive: false, autoDelete: false, arguments: null);

var message = string.Empty;
while (true)
{
	Console.WriteLine("Napisz exit aby wyjść");
	Job job = new();

	Console.WriteLine("Napisz query do wysłania");
	message = Console.ReadLine();
	if (message == "exit")
		break;
	job.Message = message;
	Console.WriteLine("Wybierz gdzie ma wysłać query? 1 - SQL | 2 - CosmosDb");
	message = Console.ReadLine();
	job.Type = Convert.ToInt32(message) == 1 ? TypeEnum.SqlQuery : TypeEnum.CosmosDbQuery;
	Console.WriteLine("Napisz czas realizacji query w [s]");
	job.TimeOfJob = Convert.ToInt32(Console.ReadLine());


	message = JsonConvert.SerializeObject(job);

	var body = Encoding.UTF8.GetBytes(message);

	var properties = new BasicProperties
	{
		Persistent = true
	};

	await channel.BasicPublishAsync(exchange: "RabbitMQ.MultiReciverOneQueue", routingKey: "", mandatory: true, basicProperties: properties, body: body);


	Console.WriteLine("");
	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("\tSend {0}", message);
	Console.ForegroundColor = ConsoleColor.Gray;
	Console.WriteLine("");
}