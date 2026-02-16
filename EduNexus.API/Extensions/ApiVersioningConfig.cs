using Asp.Versioning;

namespace EduNexus.API.Extensions
{
    public static class ApiVersioningConfig
    {
        public static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                // ========= Different ways to read the API version =========

                // 1️⃣ URL Segment versioning: /api/v1/products
                options.ApiVersionReader = new UrlSegmentApiVersionReader();

                // 2️⃣ Query String versioning: /api/products?api-version=1.0
                //options.ApiVersionReader = new QueryStringApiVersionReader();

                // 3️⃣ Header versioning: pass version in a custom header
                // Example: api-version: 1.0
                //options.ApiVersionReader = new HeaderApiVersionReader("api-version");

                // 4️⃣ Media Type (content negotiation) versioning:
                // Example: Accept: application/json; version=1.0
                //options.ApiVersionReader = new MediaTypeApiVersionReader("version");

            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = false;
            });

            return services;
        }

    }
}
