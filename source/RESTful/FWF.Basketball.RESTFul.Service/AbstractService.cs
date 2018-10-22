using Autofac;
using FWF.Basketball.Logic.Data;
using FWF.Basketball.RESTFul.Service.v1;
using FWF.Basketball.RESTFul.Service.v1.Controllers;
using FWF.Configuration;
using FWF.Logging;
using FWF.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace FWF.Basketball.RESTFul.Service
{
    internal abstract class AbstractService : Startable
    {
        private IWebHost _webHost;
        private Uri _hostUri;

        private readonly IComponentContext _componentContext;
        private readonly ILocalTcpPortManager _localTcpPortManager;
        private readonly IAppSettings _appSettings;
        private readonly ILog _log;

        public AbstractService(
            IComponentContext componentContext,
            ILogFactory logFactory
            )
        {
            _componentContext = componentContext;

            _localTcpPortManager = componentContext.Resolve<ILocalTcpPortManager>();
            _appSettings = componentContext.Resolve<IAppSettings>();

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            // Update some basic settings
            _appSettings.AddOrUpdate(CommonAppSettingsKeys.ClientIdSetting.Name, Guid.NewGuid().ToString());
            _appSettings.AddOrUpdate(CommonAppSettingsKeys.ClientNameSetting.Name, "RESTFul Service");

            // Start dependent components

            // Get a TCP port that is available
            var portReservation = _localTcpPortManager.GetNextAvailablePort();

            var hostPort = portReservation.Port;
            var sslPort = portReservation.Port + 1;

            // Update the local setting to begin hosting with the port
            _appSettings.AddOrUpdate(CommonAppSettingsKeys.HttpHostPortSetting.Name, portReservation.Port.ToString());


            try
            {
                // Expose API endpoint
                if (_webHost.IsNotNull())
                {
                    _webHost.Dispose();
                }

                var builder = WebHost.CreateDefaultBuilder();

                //
                _hostUri = new UriBuilder(
                    "https",
                    "localhost",
                    sslPort
                    ).Uri;

                builder.UseKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 100;
                    options.Limits.MaxConcurrentUpgradedConnections = 100;
                    options.Limits.MaxRequestBodySize = 10 * 1024;
                    options.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    options.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));

                    options.Listen(IPAddress.Loopback, hostPort);
                    options.Listen(IPAddress.Loopback, sslPort, listenOptions =>
                    {
                        listenOptions.UseHttps(StoreName.My, "localhost");
                    });
                }
                );

                builder.ConfigureServices(ConfigureServices);
                builder.Configure(Configure);

                _webHost = builder.Build();

                try
                {
                    _webHost.Start();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);

                    throw;
                }
            }
            finally
            {
                portReservation.Dispose();
            }

            _log.InfoFormat(
                "Listening on: http://localhost:{0}/ and https://localhost:{1}/",
                hostPort,
                sslPort
                );
        }

        protected override void OnStop()
        {
            _webHost.Dispose();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // 
            services.AddSingleton(_componentContext.Resolve<IRESTFulGameEngineListener>());
            services.AddSingleton(_componentContext.Resolve<IGameDataRepository>());
            services.AddSingleton(_componentContext.Resolve<IAppSettings>());
            services.AddSingleton(_componentContext.Resolve<ILogFactory>());

            //
            services.AddTransient<HomeController>();
            services.AddTransient<GameController>();
            services.AddTransient<PlayerController>();
            services.AddTransient<ScoreController>();

            // 
            services.AddAuthentication();

            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddAuthorization()
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddApiExplorer()
                .AddApplicationPart(typeof(Program).Assembly);

        }

        private void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseResponseBuffering();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }

    }
}

