using EasyNetQ;
using System;
using Messages;

namespace Publisher
{
    public class Program
    {
        const string Amqp = "amqps://gsfihevq:eFtfoAM2d-JvSPxFBMu8te_VYbO91cBN@bunny.rmq.cloudamqp.com/gsfihevq";

        public static void Main(string[] args)
        {
            using IBus bus = RabbitHutch.CreateBus(Amqp);
            int count = 1;
            while (true)
            {
                Console.WriteLine("Press 1 to publish a string, or 2 to publish a message");
                ConsoleKeyInfo key = Console.ReadKey(false);
                string message = $"Greeting {count++} from {Environment.MachineName} at {DateTime.UtcNow:O}";
                switch (key.KeyChar)
                {
                    case '1':
                        bus.PubSub.Publish(message);

                        Console.WriteLine($"Published message: {message}");

                        break;

                    case '2':
                        Greeting greeting = new Greeting
                        {
                            From = "Dylan",
                            Text = message,
                            Language = "English"
                        };

                        bus.PubSub.Publish(greeting);
                        
                        Console.WriteLine($"Published a greeting!");

                        break;

                    default:
                        Console.WriteLine("That's not a valid message key.");
                        break;
                }
            }
        }
    }
}
