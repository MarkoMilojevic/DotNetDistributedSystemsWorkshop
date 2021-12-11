using EasyNetQ;
using System;

namespace Publisher
{
    public class Program
    {
        const string Amqp = "amqps://gsfihevq:eFtfoAM2d-JvSPxFBMu8te_VYbO91cBN@bunny.rmq.cloudamqp.com/gsfihevq";

        public static void Main(string[] args)
        {
            IBus bus = RabbitHutch.CreateBus(Amqp);

            int count = 1;
            while (true)
            {
                Console.WriteLine("Press a key to publish a message...");
                Console.ReadKey(false);
                string message = $"Message {count++} from {Environment.MachineName} at {DateTime.UtcNow:O}";
                bus.PubSub.Publish(message);
                Console.WriteLine($"Published message: {message}");
            }
        }
    }
}
