
namespace FWF.Logging
{
    public interface ILogFactory : IRunnable
    {
        ILog CreateForType(object instance);

    }
}


