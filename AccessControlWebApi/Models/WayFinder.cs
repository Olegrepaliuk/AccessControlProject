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
        private HashSet<int> CurrentWay;
        private List<HashSet<int>> Ways;
        private ControlRepository repo;

        public WayFinder()
        {
            Visited = new HashSet<int>();
            CurrentWay = new HashSet<int>();
            Ways = new List<HashSet<int>>();
        }
        public bool DeleteAbility(int id, int deleteRoomId)
        {
            CurrentWay.Clear();
            Visited.Clear();
            Ways.Clear();
            var room = repo.GetRoomById(id);
            Search(room);
            return Check(deleteRoomId);
        }
        private void Search(Room room)
        {
            if (Visited.Contains(room.Id))
            {
                return;
            }
            Visited.Add(room.Id);
            CurrentWay.Add(room.Id);
            if(room.Type == RoomType.Hall)
            {
                Ways.Add(new HashSet<int>(CurrentWay));
                return;
            }
            else
            {
                List<Room> neighbourRooms = repo.GetNeighbourRooms(room.Id).ToList();
                foreach (var item in neighbourRooms)
                {
                    Search(item);                   
                }
            }
            CurrentWay.Remove(room.Id);
        }

        private bool Check(int idToRemove)
        {
            int otherWaysCounter = 0;
            foreach (var item in Ways)
            {
                if(!item.Contains(idToRemove))
                {
                    otherWaysCounter++;
                }
            }
            if (otherWaysCounter == 0) return false;
            return true;
        }
    }
}
