using Autofac;

namespace FWF.Bootstrap
{
    public class RootModule : Module
    {

        private static System.Reflection.Assembly _assembly = typeof(RootModule).Assembly;

        protected override void Load(ContainerBuilder builder)
        {
        }
    }
}



