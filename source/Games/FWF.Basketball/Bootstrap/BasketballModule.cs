using Autofac;
using FWF.Basketball.Logging;
using FWF.Logging;

namespace FWF.Basketball.Bootstrap
{
    public class BasketballModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleLogFactory>()
                .AsSelf()
                .As<ILogFactory>()
                .SingleInstance();
        }
    }
}



