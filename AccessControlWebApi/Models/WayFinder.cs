using AccessControlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class WayFinder
    {
        private HashSet<int> Visited;
        private List<List<int>> Ways;
        private ControlRepository repo;
        public void Find(int id, int deleteRoomId)
        {
            

        }
        private Room Search(int id)
        {
            if (Visited.Contains(id))
            {
                return null;
            }
            Visited.Add(id);
            var room = repo.GetRoomById(id);
            if(room.Type == RoomType.Hall)
            {
                return room;
            }
            else
            {
                List<Room> neighbourRooms = repo.GetNeighbourRooms(id).ToList();
                foreach (var item in neighbourRooms)
                {
                    var result = Search(item.Id);
                    if(result != null)
                    {

                    }
                }
            }

        }
    }
}
