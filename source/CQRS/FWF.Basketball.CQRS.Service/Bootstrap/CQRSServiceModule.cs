using Autofac;
using FWF.Basketball.CQRS.Service.Web;

namespace FWF.Basketball.CQRS.Service.Bootstrap
{
    public class CQRSServiceModule : Module
    {
        private static readonly System.Reflection.Assembly _assembly = typeof(CQRSServiceModule).Assembly;

        protected override void Load(ContainerBuilder builder)
        {
            // Register all handlers
            builder.RegisterAssemblyTypes(_assembly)
                .Where(x => typeof(IHttpRequestHandler).IsAssignableFrom(x))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();

        }
    }
}
