using Autofac;
using FWF.Logging;
using FWF.CQRS;
using FWF.Data.Local;
using FWF.Net;
using FWF.Threading;
using FWF.Configuration;
using FWF.Json;

namespace FWF.Bootstrap
{
    public class RootModule : Module
    {

        private static System.Reflection.Assembly _assembly = typeof(RootModule).Assembly;

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterBuildCallback(x => {

                // Forward the container to the static JSON component
                JSON.ComponentContext = x;
            });

            builder.RegisterType<CqrsLogicHandler>()
                .AsSelf()
                .As<ICqrsLogicHandler>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(CommandHandlerItem<>))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(QueryHandlerItem<>))
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(EventHandlerItem<>))
                .AsSelf()
                .InstancePerDependency();



            builder.RegisterType<EventPublisher>()
                .AsSelf()
                .As<IEventPublisher>()
                .SingleInstance();

            builder.RegisterType<LocalDataContext>()
                .AsSelf()
                .As<ILocalDataContext>()
                .InstancePerDependency();

            builder.RegisterType<LocalTcpPortManager>()
                .AsSelf()
                .As<ILocalTcpPortManager>()
                .SingleInstance();

            builder.RegisterType<InMemoryAppSettings>()
                .AsSelf()
                .As<IAppSettings>()
                .SingleInstance();

            builder.RegisterType<StartableThread>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<NoOpLogFactory>()
                .AsSelf()
                .As<ILogFactory>()
                .SingleInstance();

            builder.RegisterType<RngRandom>()
                .AsSelf()
                .As<IRandom>()
                .SingleInstance();



            builder.RegisterType<JsonSerializer>()
                .AsSelf()
                .As<IJsonSerializer>()
                .InstancePerDependency();
            builder.RegisterType<JsonReader>()
                .AsSelf()
                .As<IJsonReader>()
                .InstancePerDependency();
            builder.RegisterType<JsonWriter>()
                .AsSelf()
                .As<IJsonWriter>()
                .InstancePerDependency();


            builder.RegisterAssemblyTypes(_assembly)
                .Where(x => typeof(IJsonConverter).IsAssignableFrom(x))
                .AsSelf()
                .As<IJsonConverter>()
                .SingleInstance();


            builder.RegisterType<DateTimeClock>()
                .AsSelf()
                .As<IClock>()
                .As<IAcceleratedClock>()
                .InstancePerDependency();

            builder.RegisterType<DateTimeNow>()
                .AsSelf()
                .As<INow>()
                .SingleInstance();

        }

    }
}



