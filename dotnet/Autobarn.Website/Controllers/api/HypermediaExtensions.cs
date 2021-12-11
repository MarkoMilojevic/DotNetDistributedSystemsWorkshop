using Autobarn.Data.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace Autobarn.Website.Controllers.api
{
    public static class HypermediaExtensions
    {
        public static Dictionary<string, object> Paginate(
            string baseUrl,
            int index,
            int count,
            int total)
        {
            Dictionary<string, object> links = new Dictionary<string, object>
            {
                ["self"] = new { href = $"{baseUrl}?index={index}&count={count}" },
                ["first"] = new { href = $"{baseUrl}?index=0&count={count}" },
            };

            if (index > 0)
            {
                links["prev"] = new { href = $"{baseUrl}?index={Math.Max(index - count, 0)}&count={count}" };
            }

            if (index + count < total)
            {
                links["next"] = new { href = $"{baseUrl}?index={index + count}&count={count}" };
            }

            links["final"] = new { href = $"{baseUrl}?index={total - count}&count={count}" };

            return links;
        }
        public static Dictionary<string, object> PaginateByLicensePlate(
            string baseUrl,
            char index)
        {
            Dictionary<string, object> links = new Dictionary<string, object>
            {
                ["self"] = new { href = $"{baseUrl}/registration/{index}" },
                ["first"] = new { href = $"{baseUrl}/registration/A" },
            };

            if (index > 'A')
            {
                string prev = ((char)Math.Max(index - 1, 'A')).ToString();
                links["prev"] = new { href = $"{baseUrl}/registration/{prev}" };
            }

            if (index < 'Z')
            {
                string next = ((char)(index + 1)).ToString();
                links["next"] = new { href = $"{baseUrl}/registration/{next}" };
            }

            links["final"] = new { href = $"{baseUrl}/registration/Z" };

            return links;
        }

        public static dynamic ToDynamic(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj.GetType());
            foreach (PropertyDescriptor prop in properties)
            {
                if (Ignore(prop))
                {
                    continue;
                }

                expando.Add(prop.Name, prop.GetValue(obj));
            }

            return expando;
        }

        private static bool Ignore(PropertyDescriptor prop) =>
            prop.Attributes.OfType<JsonIgnoreAttribute>().Any();

        public static dynamic ToHypermediaResource(this Vehicle vehicle, string expand = "")
        {
            dynamic result = vehicle.ToDynamic();
            result._links = new
            {
                self = new
                {
                    href = $"/api/vehicles/{vehicle.Registration}",
                },
                model = new
                {
                    href = $"/api/models/{vehicle.ModelCode}",
                }
            };

            if (expand == "model")
            {
                result._embedded = new { model = vehicle.VehicleModel };
            }

            return result;
        }

        public static dynamic ToHypermediaResource(this Model model)
        {
            dynamic result = model.ToDynamic();

            result._links = new
            {
                self = new
                {
                    href = $"/api/models/{model.Code}",
                },
            };

            result._actions = new
            {
                create = new
                {
                    href = $"/api/models/{model.Code}",
                    method = "POST",
                    type = "application/json",
                    name = $"Create a new {model.Manufacturer.Name} {model.Name}",
                },
            };

            return result;
        }
    }
}
