using System.Collections.Generic;

namespace FWF.Basketball.RESTFul.App.Models
{
    public class GamesModel
    {

        public GamesModel()
        {
            this.Games = new List<GameDetail>();
        }

        public List<GameDetail> Games
        {
            get; set;
        }
    }
}
