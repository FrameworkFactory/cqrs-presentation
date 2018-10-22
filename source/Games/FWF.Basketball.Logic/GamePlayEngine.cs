using FWF.Basketball.Logic.Data;
using FWF.Configuration;
using FWF.Data.Local;
using FWF.Logging;
using FWF.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FWF.Basketball.Logic
{
    internal class GamePlayEngine : Startable, IGamePlayEngine
    {

        private List<IGamePlayListener> _listeners = new List<IGamePlayListener>();
        private volatile object _lockObject = new object();

        private List<GameMetadata> _games = new List<GameMetadata>();

        private TimeSpan _lastElapsedTime = TimeSpan.Zero;

        private readonly StartableThread _startableThread;

        private readonly IAcceleratedClock _clock;
        private readonly IGameDataRepository _gameDataRepository;
        private readonly IAppSettings _appSettings;
        private readonly IRandom _random;
        private readonly ILog _log;

        private class GameMetadata
        {
            private Game _game;
            private GameClock _gameClock = new GameClock();

            public GameMetadata(Game game)
            {
                _game = game;
            }

            public Game Game
            {
                get { return _game; }
            }

            public GameClock GameClock
            {
                get { return _gameClock; }
            }
        }

        public GamePlayEngine(
            IAcceleratedClock clock,
            StartableThread startableThread,
            IGameDataRepository gameDataRepository,
            IAppSettings appSettings,
            IRandom random,
            ILogFactory logFactory
            )
        {
            _clock = clock;
            _startableThread = startableThread;
            _gameDataRepository = gameDataRepository;
            _appSettings = appSettings;
            _random = random;

            _log = logFactory.CreateForType(this);

            // Define parameters for the background thread
            _startableThread.Name = "GameEngine";
            _startableThread.ThreadLatency = TimeSpan.FromMilliseconds(250);
            _startableThread.ThreadLoop = RunEngine;
        }

        protected override void OnStart()
        {
            _gameDataRepository.Start();

            // Initialize data in a in memory repository
            using (var writeContext = _gameDataRepository.BeginWrite())
            {
                InitTeamAndPlayerData(writeContext);
                InitGameSchedule(writeContext);
            }

            // Load all games and start the clock
            var allGames = _gameDataRepository.GetAll<Game>();

            
            foreach (var game in allGames)
            {
                var activeGame = new GameMetadata(game);

                _games.Add(activeGame);
            }

            // 
            _clock.AccelerationFactor = 2m;
            _clock.Start();

            //
            _startableThread.Start();
        }

        protected override void OnStop()
        {
            _startableThread.Stop();

            _games.Clear();
            _gameDataRepository.Stop();
        }

        private void InitTeamAndPlayerData(ILocalWriteDataContext writeContext)
        {
            var uncTeam = new Team
            {
                Id = Guid.NewGuid(),
                Name = Consts.TeamName_UNC,
            };
            writeContext.Insert(uncTeam);

            var dukeTeam = new Team
            {
                Id = Guid.NewGuid(),
                Name = Consts.TeamName_Duke,
            };
            writeContext.Insert(dukeTeam);

            var kentuckyTeam = new Team
            {
                Id = Guid.NewGuid(),
                Name = Consts.TeamName_Kentucky,
            };
            writeContext.Insert(kentuckyTeam);

            var kansasTeam = new Team
            {
                Id = Guid.NewGuid(),
                Name = Consts.TeamName_Kansas,
            };
            writeContext.Insert(kansasTeam);

            // UNC
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = uncTeam.Id,
                    Name = "Seventh Woods",
                    Position = "Guard",
                    Number = "0",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = uncTeam.Id,
                    Name = "Kenny Williams",
                    Position = "Guard",
                    Number = "24",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = uncTeam.Id,
                    Name = "Shea Rush",
                    Position = "Forward",
                    Number = "11",
                });

            // Duke
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = dukeTeam.Id,
                    Name = "Marques Bolden",
                    Position = "Center",
                    Number = "20",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = dukeTeam.Id,
                    Name = "Justin Robinson",
                    Position = "Forward",
                    Number = "50",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = dukeTeam.Id,
                    Name = "Jack White",
                    Position = "Forward",
                    Number = "41",
                });


            // Kentucky

            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = kentuckyTeam.Id,
                    Name = "Nick Richards",
                    Position = "Forward",
                    Number = "4",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = kentuckyTeam.Id,
                    Name = "PJ Washington",
                    Position = "Forward",
                    Number = "25",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = kentuckyTeam.Id,
                    Name = "Quade Green",
                    Position = "Guard",
                    Number = "0",
                });


            // Kansas

            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = kansasTeam.Id,
                    Name = "Udoka Azubuike",
                    Position = "Center",
                    Number = "35",
                });
            writeContext.Insert(
                new Player
                {
                    Id = Guid.NewGuid(),
                    TeamId = kansasTeam.Id,
                    Name = "LaGerald Vick",
                    Position = "Guard",
                    Number = "2",
                });
            writeContext.Insert(
                 new Player
                 {
                     Id = Guid.NewGuid(),
                     TeamId = kansasTeam.Id,
                     Name = "Silvio De Sousa",
                     Position = "Forward",
                     Number = "22",
                 });

        }

        private void InitGameSchedule(ILocalWriteDataContext writeContext)
        {

            var game1 = new Game
            {
                Id = Guid.NewGuid(),
                Name = "North Carolina vs. Duke",
                AwayTeamId = _gameDataRepository.FirstOrDefault<Team>(x => x.Name == Consts.TeamName_Duke).Id,
                HomeTeamId = _gameDataRepository.FirstOrDefault<Team>(x => x.Name == Consts.TeamName_UNC).Id,
            };
            writeContext.Insert(game1);

            var game2 = new Game
            {
                Id = Guid.NewGuid(),
                Name = "Kansas vs. Kentucky",
                AwayTeamId = _gameDataRepository.FirstOrDefault<Team>(x => x.Name == Consts.TeamName_Kansas).Id,
                HomeTeamId = _gameDataRepository.FirstOrDefault<Team>(x => x.Name == Consts.TeamName_Kentucky).Id,
            };
            writeContext.Insert(game2);


        }

        public void Subscribe(IGamePlayListener listener)
        {
            lock (_lockObject)
            {
                _listeners.Add(listener);
            }
        }

        public void Unsubscribe(IGamePlayListener listener)
        {
            lock (_lockObject)
            {
                _listeners.Remove(listener);
            }
        }

        private Task RunEngine(IThreadLoopEvent resetEvent)
        {
            if (!IsRunning)
            {
                resetEvent.Cancel();
                return Task.CompletedTask;
            }

            try
            {
                // Record how much time has passed
                var elapsed = _clock.Elapsed - _lastElapsedTime;

                // Save last elapsed time so that we get chunks of time - rather than an increased total time
                _lastElapsedTime = _clock.Elapsed;

                // Increment game clocks
                foreach (var gameMeta in _games)
                {
                    if (resetEvent.IsCancelled)
                    {
                        break;
                    }

                    var gameClock = gameMeta.GameClock;

                    // If game is complete - then skip
                    if (gameClock.IsComplete)
                    {
                        continue;
                    }

                    // 
                    IncrementGameClock(gameClock, elapsed);

                    //_log.InfoFormat(
                    //    "{0} - Q{1} {2}",
                    //    game.Name,
                    //    gameClock.Quarter,
                    //    gameClock.Time
                    //    );

                    // Push changes to any listener
                    lock (_lockObject)
                    {
                        foreach (var listener in _listeners)
                        {
                            listener.GameClockChange(
                                gameMeta.Game,
                                gameMeta.GameClock
                                );
                        }
                    }
                }

                // Add possible scores
                foreach (var gameMeta in _games)
                {
                    if (resetEvent.IsCancelled)
                    {
                        break;
                    }

                    // If game is complete - then skip
                    if (gameMeta.GameClock.IsComplete)
                    {
                        continue;
                    }

                    var score = AddPossibleGameScore(gameMeta.Game);

                    if (score.IsNull())
                    {
                        continue;
                    }

                    _log.InfoFormat(
                        "{0} - {1}, +{2}",
                        gameMeta.Game.Name,
                        score.PlayerName,
                        score.Points
                        );

                    // Push changes to any listener
                    lock (_lockObject)
                    {
                        foreach (var listener in _listeners)
                        {
                            listener.ScoreChange(gameMeta.Game, gameMeta.GameClock, score);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }

            return Task.CompletedTask;
        }

        private void IncrementGameClock(GameClock gameClock, TimeSpan elapsed)
        {
            // Rather than have every game increment time in the same sequence - add some variability
            var variableTime = TimeSpan.FromMilliseconds(_random.Any(-(int)elapsed.TotalMilliseconds, (int)elapsed.TotalMilliseconds));

            // Add more variable time, if it is above 0
            var gameElapsed = elapsed + (variableTime > TimeSpan.Zero ? variableTime : TimeSpan.Zero);

            //
            var newClockTime = gameClock.Time - gameElapsed;

            // Ensure the clock is rounded to the nearest tenth of a second
            newClockTime = newClockTime.RoundToNearest(TimeSpan.FromMilliseconds(100));

            // Check if time has expired for the current quarter
            if (newClockTime > TimeSpan.Zero)
            {
                gameClock.Time = newClockTime;
            }
            else
            {
                //
                if (gameClock.Quarter >= GameClock.NumberOfQuarters)
                {
                    // Game is over
                    gameClock.Time = TimeSpan.Zero;
                    return;
                }

                gameClock.Quarter++;
                gameClock.Time = GameClock.TimeEachQuarter + newClockTime; // In this case, new time is negative
            }
        }

        private Score AddPossibleGameScore(Game game)
        {
            // In order to determine if the latest game change is a scoring event, we create a
            // weighted decision based upon the last time a score was made for the game

            var timeSinceLastScore = _clock.Now - game.LastScoreTimestamp;

            var randomSeconds = TimeSpan.FromSeconds(_random.Any(1, 10));

            if ((timeSinceLastScore + randomSeconds) < TimeSpan.FromSeconds(10))
            {
                return null;
            }




            // Randomly choose if the home/away team is scoring
            var teamId = _random.AnyBoolean() ? game.HomeTeamId : game.AwayTeamId;

            // Get any player who made the score
            var player = _gameDataRepository.GetRandom<Player>(x => x.TeamId == teamId);

            // Determine number of points
            var points = _random.Any(1, 3);

            var score = new Score
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                GameName = game.Name,
                TeamId = teamId,
                TeamName = null,
                PlayerId = player.Id,
                PlayerName = player.Name,
                PlayerPosition = player.Position,
                Points = points,
            };

            // Record the last time a score was made in the game
            game.LastScoreTimestamp = _clock.Now;

            using (var writeContext = _gameDataRepository.BeginWrite())
            {
                writeContext.Update(game);
                writeContext.Insert(score);
            }

            return score;
        }

    }
}
