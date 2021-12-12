using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes
{
    public sealed class VehicleModelGraphType : ObjectGraphType<Model>
    {
        public VehicleModelGraphType()
        {
            Name = "model";
            Field(m => m.Name).Description("The name of this model of vehicle");
            Field(m => m.Code).Description("The unique database code identifying this model");
            Field(m => m.Manufacturer, type: typeof(ManufacturerGraphType))
                .Description("Which company manufactures this model of vehicle?");
        }
    }
}
