using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAgainstHumanity.Models.Db
{
    [Table("cardset")]
    public class CardSet
    {
        [ColumnAttribute("cdsid")]
        public int Id { get; set; }
        [ColumnAttribute("cdsguid")]
        public string Guid { get; set; }
        [ColumnAttribute("cdsname")]
        public string Name { get; set; }
        [ColumnAttribute("cdsauthor")]
        public string Author { get; set; }
        [ColumnAttribute("cdsadded")]
        public DateTime Added { get; set; }
        [ColumnAttribute("cdslang")]
        public string Language { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
    }
}