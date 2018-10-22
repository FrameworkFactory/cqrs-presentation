using Autofac;
using FWF.Basketball.Logic;

namespace FWF.Basketball.Console.Bootstrap
{
    public class ConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NoOpGamePlayListener>()
                .AsSelf()
                .As<IGamePlayListener>()
                .SingleInstance();
                
        }
    }
}
