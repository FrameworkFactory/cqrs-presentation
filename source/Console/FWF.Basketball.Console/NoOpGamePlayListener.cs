using FWF.Basketball.Logic;

namespace FWF.Basketball.Console
{
    internal class NoOpGamePlayListener : IGamePlayListener
    {


        public void GameClockChange(Game game, GameClock gameClock)
        {
            // Do nothing
        }

        public void ScoreChange(Game game, GameClock gameClock, Score score)
        {
            // Do nothing
        }


    }
}
