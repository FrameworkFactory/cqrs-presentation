using FWF.Basketball.Logic;
using System;

namespace FWF.Basketball.RESTFul
{
    public interface IRESTFulGameEngineListener : IGamePlayListener
    {

        GameClock GetGameClockById(Guid gameId);

    }
}
