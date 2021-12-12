using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes
{
    public sealed class ManufacturerGraphType : ObjectGraphType<Manufacturer>
    {
        public ManufacturerGraphType()
        {
            this.Name = "manufacturer";
            this.Field(c => c.Name).Description("The name of the manufacturer, e.g. Tesla, Volkswagen, Ford");
            this.Field(c => c.Code).Description("The unique database code identifying this manufacturer");
        }
    }
}
