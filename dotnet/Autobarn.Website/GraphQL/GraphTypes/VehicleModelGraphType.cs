using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes
{
    public sealed class VehicleModelGraphType : ObjectGraphType<Model>
    {
        public VehicleModelGraphType()
        {
            this.Name = "model";
            this.Field(m => m.Name).Description("The name of this model of vehicle");
            this.Field(m => m.Code).Description("The unique database code identifying this model");
            this.Field(m => m.Manufacturer, type: typeof(ManufacturerGraphType))
                .Description("Which company manufactures this model of vehicle?");
        }
    }
}
