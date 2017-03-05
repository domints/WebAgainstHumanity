using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebAgainstHumanity.Models;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Managers
{
    interface ICardsManager
    {
        List<CardSet> GetAll();
        List<Card> GetCardsForCardSet(CardSet cardSet);
    }

    class CardsManager : ICardsManager
    {
        private ConcurrentDictionary<string, CardSetCache> _cards = new ConcurrentDictionary<string, CardSetCache>();
        private ConcurrentDictionary<string, object> _cardsLocks = new ConcurrentDictionary<string, object>();
        private WahContext cx;
        private object genericLock;

        public CardsManager(WahContext context)
        {
            cx = context;
            genericLock = new object();
        }

        public List<CardSet> GetAll()
        {
            int dbCount = cx.CardSets.Count();
            int cacheCount = _cards.Count;
            if(dbCount != cacheCount)
            {
                var css = cx.CardSets.ToList();
                foreach(var cs in css)
                {
                    _cards.AddOrUpdate(
                        cs.Guid,
                    new CardSetCache {
                        Id = cs.Guid,
                        CardSet = cs,
                        HasCardsLoaded = false
                    },
                    (id, old) => 
                    {
                       old.CardSet = cs;
                       return old; 
                    });
                }
            }

            return _cards.Select(c => c.Value.CardSet).ToList();
        }

        public List<Card> GetCardsForCardSet(CardSet cardSet)
        {
            CardSetCache cache;
            var success = _cards.TryGetValue(cardSet.Guid, out cache);
            if(!success) return new List<Card>();

            object Lock;
            lock(genericLock)
            {
                bool getL = _cardsLocks.TryGetValue(cardSet.Guid, out Lock);
                if(!getL)
                {
                    Lock = new object();
                    _cardsLocks.TryAdd(cardSet.Guid, Lock);
                }
            }

            lock(Lock)
            {
                if(!cache.HasCardsLoaded)
                {
                    var cards = cx.Cards.Where(cd => cd.CardSet == cardSet).ToList();
                    cache.CardSet.Cards = cards;
                    cache.HasCardsLoaded = true;
                }
            }

            return cache.CardSet.Cards.ToList();
        }
    }
}