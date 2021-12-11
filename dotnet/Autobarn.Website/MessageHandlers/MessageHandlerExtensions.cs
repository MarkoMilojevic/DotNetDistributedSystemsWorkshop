using Autobarn.Data.Entities;
using Autobarn.Messages;
using EasyNetQ;
using System;

namespace Autobarn.Website.MessageHandlers
{
    public static class MessageHandlerExtensions
    {
        public static void PublishNewVehicleMessage(this IBus bus, Vehicle vehicle)
        {
            NewVehicleMessage message = new NewVehicleMessage
            {
                Registration = vehicle.Registration,
                Color = vehicle.Color,
                Year = vehicle.Year,
                Manufacturer = vehicle?.VehicleModel?.Manufacturer.Name,
                ModelName = vehicle?.VehicleModel?.Name,
                ListedAt = DateTimeOffset.UtcNow
            };

            bus.PubSub.Publish(message);
        }
    }
}
