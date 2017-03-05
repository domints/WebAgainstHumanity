using System.Collections.Generic;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Models
{
    public class GameRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public User Creator { get; set; }
        public List<CardSet> CardSet { get; set; }
    }
}