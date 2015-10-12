using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lisa.Common.WebApi.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.AddSingleton<Database>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}