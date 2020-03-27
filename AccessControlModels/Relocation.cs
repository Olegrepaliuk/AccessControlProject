using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlModels
{
    public class Relocation
    {
        public int Id { get; set; }
        public Room FromLoc { get; set; }
        public int? FromLocId { get; set; }
        public Room ToLoc { get; set; }
        public int? ToLocId { get; set; }
        public DateTime DateAndTime { get; set; }
        public Person Person { get; set; }
        public int PersonId { get; set; }
        public bool Success { get; set; }
    }
}
