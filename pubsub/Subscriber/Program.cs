﻿using EasyNetQ;
using System;

namespace Subscriber
{
    public class Program
    {
        const string Amqp = "amqps://gsfihevq:eFtfoAM2d-JvSPxFBMu8te_VYbO91cBN@bunny.rmq.cloudamqp.com/gsfihevq";

        public static void Main(string[] args)
        {
            using IBus bus = RabbitHutch.CreateBus(Amqp);

            string subscriptionId = "itkonekt";
            bus.PubSub.Subscribe<string>(subscriptionId, (string message) =>
            {
                Console.WriteLine(message);
            });

            Console.WriteLine("Subscribed. Listening for messages...");
            Console.ReadLine();
        }
    }
}
