namespace RabbitMQ.Shared
{
	public class Job
	{
		public string Message { get; set; }
		public TypeEnum Type { get; set; }
		public int TimeOfJob { get; set; }
	}
}
