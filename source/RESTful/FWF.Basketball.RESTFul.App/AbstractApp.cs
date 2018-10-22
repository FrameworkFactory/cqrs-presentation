using Autofac;
using FWF.Configuration;
using FWF.Logging;
using FWF.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace FWF.Basketball.RESTFul.App
{
    public abstract class AbstractApp : Startable
    {
        private IWebHost _webHost;
        private Uri _hostUri;

        private readonly IComponentContext _componentContext;
        private readonly ILocalTcpPortManager _localTcpPortManager;
        private readonly IAppSettings _appSettings;
        private readonly ILog _log;

        public AbstractApp(
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

                var builder = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder();

                //
                _hostUri = new UriBuilder(
                    "https",
                    "localhost",
                    sslPort
                    ).Uri;

                builder.UseKestrel(options =>
                {
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

        public IWebHost WebHost
        {
            get { return _webHost; }
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_componentContext.Resolve<IAppSettings>());
            services.AddSingleton(_componentContext.Resolve<ILogFactory>());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseResponseBuffering();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}

