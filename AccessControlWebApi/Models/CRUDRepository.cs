using AccessControl.Models;
using AccessControlModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class CRUDRepository
    {
        private AccessCtrlContext db;

        public CRUDRepository(AccessCtrlContext context)
        {
            db = context;
        }

        public IEnumerable<Person> People
        {
            get
            {
                return db.People;
            }

        }

        public IEnumerable<Room> Rooms
        {
            get
            {
                return db.Rooms;
            }
        }

        public IEnumerable<Building> Buildings
        {
            get
            {
                return db.Buildings;
            }
        }

        public void PutPerson(Person person)
        {
            db.Entry(person).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Person GetPersonById(int id)
        {
            //return db.People.Where(p => p.Id == id).FirstOrDefault();
            return db.People.Find(id);
        }

        public void AddPerson(Person person)
        {
            db.People.Add(person);
            db.SaveChanges();
        }

        public string DeletePerson(int id)
        {
            //var person = db.People.Where(p => p.Id == id).FirstOrDefault();
            var person = db.People.Find(id);
            if (person != null)
            {
                db.People.Remove(person);
                db.SaveChanges();
                return "deleted";
            }
            else
            {
                return "NotFound";
            }

        }

        public Room GetRoomById(int id)
        {
            return db.Rooms.Include(r => r.Building).Where(r => r.Id == id).FirstOrDefault();
            //return db.Rooms.Where(r => r.Id == id).FirstOrDefault();
        }

        public void ConnectRooms(int id, int? connectedRoomId)
        {
            AddDoor(id, connectedRoomId);
        }

        public void AddRoom(Room room)
        {
            db.Rooms.Add(room);
            db.SaveChanges();
        }
        public void PutRoom(Room room)
        {
            db.Entry(room).State = EntityState.Modified;
            db.SaveChanges();
        }
        public string DeleteRoom(int id)
        {
            //var room = db.Rooms.Where(r => r.Id == id).FirstOrDefault();
            var room = db.Rooms.Find(id);
            if (room != null)
            {
                db.Rooms.Remove(room);
                db.SaveChanges();
                return "deleted";
            }
            else
            {
                return "NotFound";
            }

        }

        public void AddBuilding(Building building)
        {
            db.Buildings.Add(building);
            db.SaveChanges();
        }

        public Building GetBuildingById(int id)
        {
            //return db.Buildings.Include(b => b.Rooms).Where(b => b.Id ==id).FirstOrDefault();
            //return db.Buildings.Where(b => b.Id == id).FirstOrDefault();
            return db.Buildings.Find(id);
        }

        public void PutBuilding(Building building)
        {
            db.Entry(building).State = EntityState.Modified;
            db.SaveChanges();
        }

        public string DeleteBuilding(int id)
        {
            //var building = db.Buildings.Where(b => b.Id == id).FirstOrDefault();
            var building = db.Buildings.Find(id);
            if (building != null)
            {
                db.Buildings.Remove(building);
                db.SaveChanges();
                return "deleted";
            }
            else
            {
                return "NotFound";
            }

        }

        public IEnumerable<PersonRoom> GetPersonRoomsAccess(int personId)
        {
            return db.PersonRoom.Where(pr => pr.PersonId == personId);
        }

        public void UpdatePersonAccess(int id, IEnumerable<int> roomsId)
        {
            var setIds = roomsId.ToHashSet();
            var pairs = GetPersonRoomsAccess(id);
            foreach (var item in pairs)
            {
                if (!setIds.Contains(item.RoomId))
                {
                    var extraEntities = db.PersonRoom.Where(pr => (pr.PersonId == item.PersonId) && (pr.RoomId == item.RoomId));
                    if (extraEntities.Count() > 0)
                    {
                        db.PersonRoom.RemoveRange(extraEntities);
                    }

                }
                else
                {
                    setIds.Remove(item.RoomId);
                }

            }

            foreach (var roomIdItem in setIds)
            {
                var personRoom = new PersonRoom(id, roomIdItem);
                db.PersonRoom.Add(personRoom);
            }
            db.SaveChanges();
        }

        public IEnumerable<Room> GetRoomsOfPersonAccess(int personId)
        {
            List<Room> resultRooms = new List<Room>();
            var personRooms = GetPersonRoomsAccess(personId);
            foreach (var item in personRooms)
            {
                var room = db.Rooms.Find(item.RoomId);
                if (room != null) resultRooms.Add(room);
            }
            return resultRooms;
        }
        public IEnumerable<Room> GetRoomsOfBuilding(int id)
        {
            return db.Rooms.Include(r => r.Building).Where(r => r.BuildingId == id);
        }

        public void ConnectRoomWithOthers(int roomId, List<int> otherRoomsIds)
        {
            Door door;
            if(otherRoomsIds.Contains(-1))
            {
                door = new Door(roomId, null);
                db.Doors.Add(door);
            }
            var existingIds = db.Rooms.Where(r => otherRoomsIds.Contains(r.Id)).Select(r=> r.Id);
            foreach (var item in existingIds)
            {
                door = new Door(roomId, item);
                db.Doors.Add(door);
            }
            db.SaveChanges();
        }

        public int? FindLastLoggedPersonLocId(int personId)
        {
            var lastLoc = db.Relocations
                                 .Where(rel => rel.PersonId == personId)
                                 .OrderByDescending(rel => rel.DateAndTime)
                                 .FirstOrDefault();
            if (lastLoc == null)
            {
                return -1;
            }
            return lastLoc.ToLocId;
            
        }

        public void AddDoor(int? firstLocId, int? secLocId)
        {
            var door = new Door(firstLocId, secLocId);
            db.Doors.Add(door);
            db.SaveChanges();
        }
    }
}
