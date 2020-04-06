using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataContext.Models;

namespace CoolStudents.Models
{
    public class StudentAPI: Student
    {
       public List<IdText> Groups { get; set; }
    }

    public class GroupAPI : Group
    {
        public int? UserCount { get; set; }
    }
}
