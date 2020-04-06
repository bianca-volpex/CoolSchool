using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataContext.Models
{
    [Table("Group")]
    public class Group
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Название должно быть до 25 символов")]
        public string Title { get; set; }

        public virtual ICollection<UserInGroup> UserInGroups { get; set; }

        public Group() {
            this.Title = "!Новая группа";
        }
    }
}
