using AccessControlModels;
using System.Collections.Generic;
using System.Linq;

namespace AccessControlWebApi.Models
{
    public class WayFinder
    {
        private HashSet<int> Visited;
        private HashSet<int> CurrentWay;
        private List<HashSet<int>> Ways;
        private ControlRepository repo;
        private int _roomIdToDelete { get; set; }

        public WayFinder(ControlRepository repository, int roomIdToDelete)
        {
            Visited = new HashSet<int>();
            CurrentWay = new HashSet<int>();
            Ways = new List<HashSet<int>>();
            repo = repository;
            _roomIdToDelete = roomIdToDelete;
        }
        public bool DeleteAbility()
        {
            List<Room> neighbourRooms = repo.GetNeighbourRooms(_roomIdToDelete).ToList();
            foreach(var item in neighbourRooms)
            {
                CurrentWay.Clear();
                Visited.Clear();
                Ways.Clear();
                if (item != null)
                {
                    Search(item);
                    if (!Check()) return false;
                }
                    
            }
            return true;
        }
        private void Search(Room room)
        {
            if (room==null||room.Id == _roomIdToDelete||Visited.Contains(room.Id))
            {
                return;
            }
            Visited.Add(room.Id);
            CurrentWay.Add(room.Id);
            if(room.Type == RoomType.Hall)
            {
                Ways.Add(new HashSet<int>(CurrentWay));
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

        private bool Check()
        {
            int otherWaysCounter = 0;
            foreach (var item in Ways)
            {
                if(!item.Contains(_roomIdToDelete))
                {
                    otherWaysCounter++;
                }
            }
            if (otherWaysCounter == 0) return false;
            return true;
        }
    }
}
