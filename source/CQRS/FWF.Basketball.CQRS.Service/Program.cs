﻿using Autofac;
using FWF.Basketball.Bootstrap;
using FWF.Basketball.CQRS.Bootstrap;
using FWF.Basketball.CQRS.Service.Bootstrap;
using FWF.Basketball.Logic.Bootstrap;
using FWF.Bootstrap;
using FWF.Configuration;
using FWF.Logging;
using System;

namespace FWF.Basketball.CQRS.Service
{
    class Program
    {
        public static string[] Args
        {
            get;
            set;
        }

        private static IContainer _container;
        private static IRunnable _serviceInstance;

        static void Main(string[] args)
        {
            Args = args;

            try
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterModule<RootModule>();

                containerBuilder.RegisterModule<BasketballModule>();
                containerBuilder.RegisterModule<BasketballLogicModule>();

                containerBuilder.RegisterModule<BasketballCQRSModule>();
                containerBuilder.RegisterModule<CQRSServiceModule>();

                containerBuilder.RegisterType<CQRSService>()
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
                    _serviceInstance = _container.Resolve<CQRSService>();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to instantiate the service component - " + ex.Message, ex);
                }

                Console.Title = _serviceInstance.GetType().Name;
                _serviceInstance.Start();


                // Wait until a key is pressed
                Console.ReadKey(true);


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
