using Autofac;
using System.Reflection;

namespace FWF.Test.Boostrap
{
    public class TestModule : Autofac.Module
    {

        private static readonly Assembly _assembly = typeof(TestModule).Assembly;

        protected override void Load(ContainerBuilder builder)
        {

            
        }
    }
}



