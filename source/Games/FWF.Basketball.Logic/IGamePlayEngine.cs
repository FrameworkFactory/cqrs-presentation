using System;

namespace FWF.Basketball.Logic
{
    public interface IGamePlayEngine : IRunnable
    {

        void Subscribe(IGamePlayListener listener);

        void Unsubscribe(IGamePlayListener listener);


    }
}
