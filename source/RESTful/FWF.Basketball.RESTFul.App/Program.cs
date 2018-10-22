using Autofac;
using FWF.Basketball.Bootstrap;
using FWF.Basketball.RESTFul.App.Bootstrap;
using FWF.Bootstrap;
using FWF.Configuration;
using FWF.Logging;
using FWF.Web.Bootstrap;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace FWF.Basketball.RESTFul.App
{
    class Program
    {
        public static string[] Args
        {
            get;
            set;
        }

        private static IContainer _container;
        private static RESTFulApp _serviceInstance;

        static void Main(string[] args)
        {
            Args = args;

            try
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterModule<RootModule>();
                containerBuilder.RegisterModule<WebModule>();

                containerBuilder.RegisterModule<BasketballModule>();
                containerBuilder.RegisterModule<RESTFulAppModule>();

                containerBuilder.RegisterType<RESTFulApp>()
                    .AsSelf()
                    .SingleInstance();

                _container = containerBuilder.Build();

                // Start logging
                var logFactory = _container.Resolve<ILogFactory>();
                logFactory.Start();

                // Start application settings
                var appSettings = _container.Resolve<IAppSettings>();
                appSettings.Start();

                try
                {
                    _serviceInstance = _container.Resolve<RESTFulApp>();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to instantiate the service component - " + ex.Message, ex);
                }

                Console.Title = _serviceInstance.GetType().Name;
                _serviceInstance.Start();


                // Wait for the web application to shutdown (copied from WaitForTokenShutdownAsync)
                var service = (IApplicationLifetime)_serviceInstance.WebHost.Services.GetService(typeof(IApplicationLifetime));
                var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                var cancellationToken = service.ApplicationStopping;
                cancellationToken.Register(delegate (object obj)
                {
                    ((TaskCompletionSource<object>)obj).TrySetResult(null);
                }, taskCompletionSource);
                taskCompletionSource.Task.Wait();


                _serviceInstance.Stop();

                logFactory.Stop();
                appSettings.Stop();

                _container.Dispose();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Fatal exception:\r\n" + ex.RenderDetailString());
                Console.ResetColor();
            }


        }
    }
}
