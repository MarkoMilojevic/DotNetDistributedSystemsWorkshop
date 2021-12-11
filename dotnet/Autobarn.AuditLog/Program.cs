using Autobarn.Messages;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Autobarn.AuditLog
{
    public class Program
    {
        private static readonly IConfigurationRoot Config = ReadConfiguration();

        public static void Main(string[] args)
        {
            string amqp = Config.GetConnectionString("AutobarnRabbitMQConnectionString");
            using IBus bus = RabbitHutch.CreateBus(amqp);
            string subscriberId = $"autobarn.auditlog@{Environment.MachineName}";
            
            bus.PubSub.Subscribe<NewVehicleMessage>(subscriberId, HandleNewVehicleMessage);

            Console.WriteLine("Running Autobarn.AuditLog. Listening for messages...");
            Console.ReadLine();
        }

        private static void HandleNewVehicleMessage(NewVehicleMessage nvm)
        {
            string csv =
                $"{nvm.Registration}," +
                $"{nvm.Manufacturer}," +
                $"{nvm.ModelName}," +
                $"{nvm.Year}," +
                $"{nvm.Color}," +
                $"{nvm.ListedAt:O}" +
                $"{Environment.NewLine}";

            File.AppendAllText("vehicles.log", csv);
            Console.WriteLine(csv);
        }

        private static IConfigurationRoot ReadConfiguration()
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
