using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace TrialSwashbuckle
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var info = new Info
            {
                Title = "Trial Swashbuckle API surface.",
                Version = "V1",
            };

            // Name is case-sensitive because SwaggerGeneratorSettings.SwaggerDocs is a Dictionary<string, Info> with
            // no specified comparer. (Not adding a document is allowed but useless.) See Swashbuckle.AspNetCore#835
            services.AddSwaggerGen(options => options.SwaggerDoc("swashbuckle", info));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swashbuckle has a default route (/swagger/{documentName}/swagger.json) but no default document or name.
            // See Swashbuckle.AspNetCore#836
            app.UseSwagger();
            app.UseMvc();
        }
    }
}
