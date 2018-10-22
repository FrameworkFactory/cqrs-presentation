using System;

namespace FWF.Basketball.CQRS
{
    public class PlayerFantasy
    {
        public Guid Id 
        {
            get; set;
        }

        public string Name 
        {
            get; set;
        }

        public Guid TeamId 
        {
            get; set;
        }

        public int FantasyPoints 
        {
            get; set;
        }


    }
}