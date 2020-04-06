using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataContext.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int GenderId { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Фамилия должна быть до 40 символов")]
        public string LastName { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Имя должно быть до 40 символов")]
        public string FirstName { get; set; }

        [StringLength(60, ErrorMessage = "Отчество  должна быть до 60 символов")]
        public string SecondName { get; set; }

        [StringLength(16, MinimumLength = 6, ErrorMessage = "Позывной должен быть от 6 до 16 символов")]
        public string CallSign { get; set; }


        public virtual ICollection<UserInGroup> UserInGroups { get; set; }
        [ForeignKey("GenderId")]
        public virtual Gender Gender { get; set; }


        public Student()
        {
            this.GenderId = 1;
            this.FirstName = "Пользователь";
            this.LastName = "!Новый";
        }

        [NotMapped]
        public string FullName { get {
                var sn = string.IsNullOrEmpty(this.SecondName) ? "" : $"  {this.SecondName}";
                return $"{this.LastName} {this.FirstName}{sn}";
            } }
    }
}
