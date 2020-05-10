using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw_3.Models
{
    public class StudentRequest
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Studies { get; set; }
    }
}