using Autofac;
using FWF.Basketball.Logic.Data;

namespace FWF.Basketball.Logic.Bootstrap
{
    public class BasketballLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<GamePlayEngine>()
                .AsSelf()
                .As<IGamePlayEngine>()
                .SingleInstance();

            builder.RegisterType<GameDataRepository>()
                .AsSelf()
                .As<IGameDataRepository>()
                .SingleInstance();
        }
    }
}



