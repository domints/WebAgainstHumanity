using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAgainstHumanity.Models.Db
{
    [Table("user")]
    public class User
    {
        [ColumnAttribute("usrid")]
        public int Id { get; set; }
        [ColumnAttribute("usrnick")]
        public string Nick { get; set; }
        [ColumnAttribute("usremail")]
        public string Email { get; set; }
        [ColumnAttribute("usrpassword")]
        public string Password { get; set; }
        [ColumnAttribute("usrcreatedate")]
        public DateTime CreationDate { get; set; }
        [ColumnAttribute("usractive")]
        public bool Active { get; set; }
    }
}