using System.ComponentModel.DataAnnotations.Schema;

namespace WebAgainstHumanity.Models.Db
{
    [Table("card")]
    public class Card
    {
        [ColumnAttribute("crdid")]
        public int Id { get; set; }
        [ColumnAttribute("crdguid")]
        public string Guid { get; set; }
        [ColumnAttribute("crdcdsid")]
        public int CardSetId { get; set; }
        [ColumnAttribute("crdcontent")]
        public string Content { get; set; }
        [ColumnAttribute("crdisquestion")]
        public bool IsQuestion { get; set; }

        public virtual CardSet CardSet { get; set; }
    }
}