using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Models
{
    public class CardSetCache
    {
        public string Id { get; set; }
        public CardSet CardSet { get; set; }
        public bool HasCardsLoaded { get; set; }
    }
}