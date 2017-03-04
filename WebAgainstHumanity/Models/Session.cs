using System;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Models
{
    public class Session
    {
        public string Id { get; set; }
        public User User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastConnection { get; set; }
        public string BrowserFootprint { get; set; }
    }
}