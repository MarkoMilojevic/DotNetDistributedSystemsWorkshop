using System;
using System.Collections.Generic;

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
    }
}
