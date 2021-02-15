using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ReadingIsGood.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;

                setup.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                setup.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                setup.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
                setup.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));
                setup.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setup.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                setup.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            });

            services.AddControllers();


            services.AddVersionedApiExplorer(setupAction => { setupAction.GroupNameFormat = "'v'VV"; });

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.ReportApiVersions = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                //setupAction.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            var apiVersionDescriptionProvider =
                services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(setupAction =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerDoc($"ReadingIsGood.API{description.GroupName}",
                        new OpenApiInfo
                        {
                            Title = "ReadingIsGood.API",
                            Version = description.ApiVersion.ToString(),
                            Description = "ReadingIsGood.API Description",
                            Contact = new OpenApiContact()
                            {
                                Email = "kudretkrt@gmail.com",
                                Name = "Kudret Kurt"
                            },
                            License = new OpenApiLicense()
                            {
                                Name = "MIT License",
                                Url = new Uri("https://opensource.org/licenses/MIT")
                            }
                        });
                }

                setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    var actionApiVersionModel =
                        apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit |
                                                                           ApiVersionMapping.Implicit);

                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                            $"ReadingIsGood.APIv{v.ToString()}" == documentName);
                    }

                    return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                        $"ReadingIsGood.APIv{v.ToString()}" == documentName);
                });

                setupAction.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        new string[] { }
                    }
                });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseMiddleware<BasicAuthMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint($"/swagger/ReadingIsGood.API{description.GroupName}/swagger.json",
                        $"{description.GroupName.ToUpperInvariant()}");
                }

                setupAction.RoutePrefix = "";
            });
        }
    }
}