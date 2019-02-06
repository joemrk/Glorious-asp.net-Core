using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GloriousCore.Models.Data.Entities
{
    [Table("tblUsers")]
    public class LoginDBO
    {
        [Key]
        public int Id { get; set; }
        public string Log { get; set; }
        public string Pass { get; set; }

    }
}
