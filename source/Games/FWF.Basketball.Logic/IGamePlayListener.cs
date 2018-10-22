
namespace FWF.Basketball.Logic
{
    public interface IGamePlayListener
    {
        void GameClockChange(Game game, GameClock gameClock);

        void ScoreChange(Game game, GameClock gameClock, Score score);
    }
}
