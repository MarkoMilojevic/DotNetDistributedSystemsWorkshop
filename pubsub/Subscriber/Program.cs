using EasyNetQ;
using Messages;
using System;

namespace Subscriber
{
    public class Program
    {
        const string Amqp = "amqps://gsfihevq:eFtfoAM2d-JvSPxFBMu8te_VYbO91cBN@bunny.rmq.cloudamqp.com/gsfihevq";

        public static void Main(string[] args)
        {
            using IBus bus = RabbitHutch.CreateBus(Amqp);
            string subscriberId = $"subscriber@{Environment.MachineName}";
            bus.PubSub.Subscribe<string>(subscriberId, message => Console.WriteLine(message));
            bus.PubSub.Subscribe<Greeting>(subscriberId, HandleGreeting);
            Console.WriteLine("Subscribed! Listening for messages...");
            Console.ReadLine();
        }

        static void HandleGreeting(Greeting g)
        {
            Console.WriteLine($"{g.From} says (in {g.Language}:");
            Console.WriteLine($"  {g.Text}");
        }
    }
}
