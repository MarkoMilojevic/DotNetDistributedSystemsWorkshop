using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes
{
    public sealed class VehicleGraphType : ObjectGraphType<Vehicle>
    {
        public VehicleGraphType()
        {
            this.Name = "vehicle";
            this.Field(v => v.Registration);
            this.Field(v => v.Year);
            this.Field(v => v.Color);
            this.Field(c => c.VehicleModel, nullable: false, type: typeof(VehicleModelGraphType))
                .Description("The model of this particular vehicle");
        }
    }
}
