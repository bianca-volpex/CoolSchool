using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataContext.Models
{
    [Table("Gender")]
    public class Gender
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
