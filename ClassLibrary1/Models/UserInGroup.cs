using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataContext.Models
{
    [Table("UserInGroup")]
    public class UserInGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey("UserId")]
        public virtual Student Student { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

    }
}
