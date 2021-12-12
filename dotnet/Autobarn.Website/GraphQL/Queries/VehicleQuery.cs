using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Autobarn.Website.GraphQL.Queries
{
    public class VehicleQuery : ObjectGraphType
    {
        private readonly IAutobarnDatabase db;

        public VehicleQuery(IAutobarnDatabase db)
        {
            this.db = db;
            this.Field<ListGraphType<VehicleGraphType>>(
                name: "Vehicles",
                description: "Query to retrieve all vehicles",
                resolve: this.GetAllVehicles);

            this.Field<VehicleGraphType>(
                name: "vehicle",
                description: "Retrieve a single vechile",
                arguments: new QueryArguments(
                    MakeNonNullStringArgument(
                        name: "registration",
                        description: "The registration plate of the vehicle you want")),
                resolve: this.GetVehicle);

            this.Field<ListGraphType<VehicleGraphType>>(
                name: "VehiclesByColor",
                description: "Retrieve all vehicles matching a particular color",
                arguments:
                    new QueryArguments(
                        MakeNonNullStringArgument(
                            name: "color",
                            description: "What color cars do you want to see?")),
                resolve: this.GetVehiclesByColor);

            this.Field<ListGraphType<VehicleGraphType>>(
                name: "VehiclesByYear",
                description: "Retrieve all vehicles based on the year of manufacture",
                arguments:
                    new QueryArguments(
                        new QueryArgument<IntGraphType>
                        {
                            Name = "manufacturedAfterYear",
                            Description = "Retrieve all vehicles manufactured after year"
                        },
                        new QueryArgument<IntGraphType>
                        {
                            Name = "manufacturedBeforeYear",
                            Description = "Retrieve all vehicles manufactured before year"
                        },
                        new QueryArgument<IntGraphType>
                        {
                            Name = "manufacturedInYear",
                            Description = "Retrieve all vehicles manufactured in year"
                        }),
                resolve: this.GetVehiclesByYear);
        }

        private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> context) =>
            this.db.ListVehicles();

        private Vehicle GetVehicle(IResolveFieldContext<object> context) =>
            this.db.FindVehicle(context.GetArgument<string>("registration"));

        private IEnumerable<Vehicle> GetVehiclesByColor(IResolveFieldContext<object> context)
        {
            string color = context.GetArgument<string>("color");

            return this.db
                    .ListVehicles()
                    .Where(v => v.Color.Contains(color, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<Vehicle> GetVehiclesByYear(IResolveFieldContext<object> context)
        {
            int? after = context.GetArgument<int?>("manufacturedAfterYear");
            int? before = context.GetArgument<int?>("manufacturedBeforeYear");
            int? exactYear = context.GetArgument<int?>("manufacturedInYear");

            var vehicles = this.db.ListVehicles();

            if (after != null)
            {
                vehicles = vehicles.Where(v => v.Year > after.Value);
            }

            if (before != null)
            {
                vehicles = vehicles.Where(v => v.Year < before.Value);
            }

            if (exactYear != null)
            {
                vehicles = vehicles.Where(v => v.Year == exactYear.Value);
            }

            return vehicles;
        }

        private static QueryArgument MakeNonNullStringArgument(string name, string description)
        {
            return new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = name,
                Description = description
            };
        }
    }
}
