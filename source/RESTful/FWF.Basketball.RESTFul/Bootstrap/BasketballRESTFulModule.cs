using Autofac;
using FWF.Basketball.Logic;

namespace FWF.Basketball.RESTFul.Bootstrap
{
    public class BasketballRESTFulModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RESTFulGameEngineListener>()
                .AsSelf()
                .As<IGamePlayListener>()
                .As<IRESTFulGameEngineListener>()
                .SingleInstance();
        }
    }
}



