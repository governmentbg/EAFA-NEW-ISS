using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using IDocumentFilter = Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter;

namespace IARA.WebHelpers.Helpers
{
    public class SwaggerFilterOutControllers : IDocumentFilter
    {
        private List<AreaType> areas;

        public SwaggerFilterOutControllers()
        {

        }

        public SwaggerFilterOutControllers(List<AreaType> areas)
        {
            this.areas = areas;
        }

        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (areas != null)
            {
                List<IEnumerable<KeyValuePair<string, OpenApiPathItem>>> paths = new List<IEnumerable<KeyValuePair<string, OpenApiPathItem>>>();

                foreach (var area in areas)
                {
                    paths.Add(swaggerDoc.Paths.Where(x => x.Key.StartsWith($"/{area}")));
                }

                var filteredPaths = paths.SelectMany(x => x).ToDictionary(x => x.Key, x => x.Value);

                foreach (var path in swaggerDoc.Paths)
                {
                    if (!filteredPaths.ContainsKey(path.Key))
                    {
                        swaggerDoc.Paths.Remove(path.Key);
                    }
                }

            }
        }
    }
}
