using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlModels
{
    public class Reader
    {
        public int Id { get; set; }
        public Room CurrentLoc { get; set; }
        public int? CurrentLocId { get; set; }
        public Room NextLoc { get; set; }
        public int? NextLocId { get; set; }
        public Reader()
        {

        }
        public Reader(int? currentLocId, int? nextLocId)
        {
            CurrentLocId = currentLocId;
            NextLocId = nextLocId;
        }
    }
}
