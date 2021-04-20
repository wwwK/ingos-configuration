﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IngosConfiguration.API
{
    /// <summary>
    /// Application start up configuration
    /// </summary>
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<IngosConfigurationApiModule>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.InitializeApplication();
        }
    }
}