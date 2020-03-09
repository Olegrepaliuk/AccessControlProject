using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlModels
{
    public class Building
    {
        public int Id { get; set; }
        public string Name { get;}
        public string City { get; set; }
        public string Address { get; set; }
        public List<Room> Rooms { get; set; }

        public Building()
        {

        }
        public void AddHall()
        {
            Room hall = new Room();
            hall.Type = RoomType.Hall;
            Rooms.Add(hall);
        }

    }
}
