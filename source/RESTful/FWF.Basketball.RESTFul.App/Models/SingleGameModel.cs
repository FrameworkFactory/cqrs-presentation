using System.Collections.Generic;

namespace FWF.Basketball.RESTFul.App.Models
{
    public class SingleGameModel
    {

        public SingleGameModel()
        {
            this.AwayPlayers = new List<PlayerDetail>();
            this.HomePlayers = new List<PlayerDetail>();
        }

        public GameDetail Game
        {
            get; set;
        }

        public List<PlayerDetail> AwayPlayers
        {
            get; set;
        }

        public List<PlayerDetail> HomePlayers
        {
            get; set;
        }

    }
}
