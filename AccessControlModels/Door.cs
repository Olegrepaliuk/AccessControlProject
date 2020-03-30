using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlModels
{
    public class Door
    {
        public int Id { get; set; }
        public Room FirstLocation { get; set; }
        public int? FirstLocationId { get; set; }
        public Room SecondLocation { get; set; }
        public int? SecondLocationId { get; set; }
        public bool? ExitToStreet { get; set; }
        public bool? ExitToHall { get; set; }

        public Door()
        {

        }
        public Door(int? firstLocId, int? secLocId)
        {
            FirstLocationId = firstLocId;
            SecondLocationId = secLocId;
        }

    }
}
