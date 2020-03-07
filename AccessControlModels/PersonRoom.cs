using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlModels
{
    public class PersonRoom
    {
        public int PersonId { get; set; }

        public int RoomId { get; set; }

        public PersonRoom(int personId, int roomId)
        {
            PersonId = personId;
            RoomId = roomId;
        }
    }
}
