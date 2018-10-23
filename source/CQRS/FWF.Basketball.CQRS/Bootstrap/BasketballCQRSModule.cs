using Autofac;
using FWF.Basketball.CQRS.Data;
using FWF.Basketball.Logic;
using FWF.CQRS;

namespace FWF.Basketball.CQRS.Bootstrap
{
    public class BasketballCQRSModule : Module
    {
        private static readonly System.Reflection.Assembly _assembly = typeof(BasketballCQRSModule).Assembly;


        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CQRSGamePlayerListener>()
                .AsSelf()
                .As<IGamePlayListener>()
                .SingleInstance();

            builder.RegisterType<ReadCacheDataRepository>()
                .AsSelf()
                .As<IReadCacheDataRepository>()
                .SingleInstance();

            // Register all handlers

            builder.RegisterAssemblyTypes(_assembly)
                .Where(x =>
                    typeof(IQueryHandler).IsAssignableFrom(x)
                    || typeof(ICommandHandler).IsAssignableFrom(x)
                    || typeof(IEventHandler).IsAssignableFrom(x)
                )
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();

        }
    }
}



