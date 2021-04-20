using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IngosConfiguration.API.Infrastructure;
using IngosConfiguration.Application;
using IngosConfiguration.Application.Contracts;
using IngosConfiguration.Domain;
using IngosConfiguration.Domain.Shared;
using IngosConfiguration.Domain.Shared.Localization;
using IngosConfiguration.Infrastructure;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;

namespace IngosConfiguration.API
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(typeof(AbpAutofacModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(IngosConfigurationApplicationModule),
        typeof(IngosConfigurationInfrastructureModule),
        typeof(AbpPermissionManagementHttpApiModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule)
    )]
    public class IngosConfigurationApiModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "IngosConfiguration";

        #region Services

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpAspNetCoreMvcOptions>(options =>
            {
                // Set dynamic api router with api version info
                options.ConventionalControllers.Create(typeof(IngosConfigurationApplicationModule).Assembly,
                    opts => { opts.RootPath = "v{version:apiVersion}"; });

                // Specify version info for framework built-in api
                options.ConventionalControllers.Create(typeof(AbpPermissionManagementHttpApiModule).Assembly,
                    opts => { opts.ApiVersions.Add(new ApiVersion(1, 0)); });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            context.Services.AddHttpClient();
            context.Services.AddHealthChecks();

            ConfigureAuditing();
            ConfigureConventionalControllers(context);
            ConfigureAuthentication(context, configuration);
            ConfigureLocalization();
            ConfigureCache();
            ConfigureVirtualFileSystem(context);
            ConfigureRedis(context, configuration, hostingEnvironment);
            ConfigureCors(context, configuration);
            ConfigureSwaggerServices(context, configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAbpRequestLocalization();

            app.UseCorrelationId();
            app.UseVirtualFiles();
            app.UseRouting();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHealthChecks("/health");

            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.DocumentTitle = "IngosConfiguration API";

                // Display latest api version by default
                //
                var provider = context.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                var apiVersionList = provider.ApiVersionDescriptions
                    .Select(i => $"v{i.ApiVersion.MajorVersion}")
                    .Distinct().Reverse();
                foreach (var apiVersion in apiVersionList)
                    options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json",
                        $"IngosConfiguration API {apiVersion?.ToUpperInvariant()}");

                var configuration = context.GetConfiguration();
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            });

            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseUnitOfWork();
            app.UseConfiguredEndpoints();
        }

        #endregion Services

        #region Methods

        private void ConfigureAuditing()
        {
            Configure<AbpAuditingOptions>(options =>
            {
                options.ApplicationName = "IngosConfiguration"; // Set the application name
                options.EntityHistorySelectors.AddAllEntities(); // Default saving all changes of entities
            });
        }

        private void ConfigureCache()
        {
            Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "IngosConfiguration:"; });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<IngosConfigurationDomainSharedModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $@"..{Path.DirectorySeparatorChar}\IngosConfiguration.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<IngosConfigurationDomainModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $@"..{Path.DirectorySeparatorChar}\IngosConfiguration.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<IngosConfigurationApplicationContractsModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $@"..{Path.DirectorySeparatorChar}\IngosConfiguration.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<IngosConfigurationApplicationModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $@"..{Path.DirectorySeparatorChar}\IngosConfiguration.Application"));
                });
        }

        private void ConfigureConventionalControllers(ServiceConfigurationContext context)
        {
            Configure<AbpAspNetCoreMvcOptions>(options => { context.Services.ExecutePreConfiguredActions(options); });

            // Use lowercase routing and lowercase query string
            context.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            context.Services.AddAbpApiVersioning(options =>
            {
                options.ReportApiVersions = true;

                options.AssumeDefaultVersionWhenUnspecified = true;

                options.DefaultApiVersion = new ApiVersion(1, 0);

                options.ApiVersionReader = new UrlSegmentApiVersionReader();

                var mvcOptions = context.Services.ExecutePreConfiguredActions<AbpAspNetCoreMvcOptions>();
                options.ConfigureAbp(mvcOptions);
            });

            context.Services.AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'VVV";

                option.AssumeDefaultVersionWhenUnspecified = true;
            });
        }

        private static void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "IngosConfiguration";
                });
        }

        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                new Dictionary<string, string>
                {
                    {"IngosConfiguration", "IngosConfiguration API"}
                },
                options =>
                {
                    // Get application api version info
                    var provider = context.Services.BuildServiceProvider()
                        .GetRequiredService<IApiVersionDescriptionProvider>();

                    // Generate swagger by api major version
                    foreach (var description in provider.ApiVersionDescriptions)
                        options.SwaggerDoc(description.GroupName, new OpenApiInfo
                        {
                            Contact = new OpenApiContact
                            {
                                Name = "Danvic Wang",
                                Email = "danvic.wang@outlook.com",
                                Url = new Uri("https://yuiter.com")
                            },
                            Description = "IngosConfiguration API",
                            Title = "IngosConfiguration API",
                            Version = $"v{description.ApiVersion.MajorVersion}"
                        });

                    options.DocInclusionPredicate((docName, description) =>
                    {
                        // Get api major version
                        var apiVersion = $"v{description.GetApiVersion().MajorVersion}";

                        if (!docName.Equals(apiVersion))
                            return false;

                        // Replace router parameter
                        var values = description.RelativePath
                            .Split('/')
                            .Select(v => v.Replace("v{version}", apiVersion));

                        description.RelativePath = string.Join("/", values);

                        return true;
                    });

                    // Let params use the camel naming method
                    options.DescribeAllParametersInCamelCase();

                    // Cancel api version parameter in swagger doc
                    options.OperationFilter<RemoveVersionFromParameter>();

                    // Inject api and dto comments
                    //
                    var paths = new List<string>
                    {
                        @"wwwroot\api-doc\IngosConfiguration.API.xml",
                        @"wwwroot\api-doc\IngosConfiguration.Application.xml",
                        @"wwwroot\api-doc\IngosConfiguration.Application.Contracts.xml"
                    };
                    GetApiDocPaths(paths, Path.GetDirectoryName(AppContext.BaseDirectory))
                        .ForEach(x => options.IncludeXmlComments(x, true));
                });
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<IngosConfigurationResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );

                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
            });
        }

        private static void ConfigureRedis(ServiceConfigurationContext context, IConfiguration configuration,
            IHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
                return;

            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            context.Services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "IngosConfiguration-Protection-Keys");
        }

        private static void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        /// <summary>
        /// Get the api description doc path
        /// </summary>
        /// <param name="paths">The xml file path</param>
        /// <param name="basePath">The site's base running files path</param>
        /// <returns></returns>
        private static List<string> GetApiDocPaths(IEnumerable<string> paths, string basePath)
        {
            var files = from path in paths
                let xml = Path.Combine(basePath, path)
                select xml;

            return files.ToList();
        }

        #endregion Methods
    }
}