using AccessControlModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class ControlRepository
    {
        private AccessCtrlContext db;
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
        public ControlRepository(AccessCtrlContext context)
        {
            db = context;
        }

        public void SaveChanges()
        {
            db.SaveChanges();
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
        public int CountPeople()
        {
            return db.People.Count();
        }

        public Room GetRoomById(int id)
        {
            return db.Rooms.Include(r => r.Building).Where(r => r.Id == id).FirstOrDefault();
            //return db.Rooms.Where(r => r.Id == id).FirstOrDefault();
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

        public void AddDoor(Door door)
        {
            db.Doors.Add(door);
            //db.SaveChanges();
        }

        public void AddPersonRoom(PersonRoom pr)
        {
            db.PersonRoom.Add(pr);
            //db.SaveChanges();
        }

        public Relocation FindLastPersonRelocation(int personId)
        {
            return db.Relocations
                                 .Where(rel => (rel.PersonId == personId)&(rel.Success == true))
                                 .OrderByDescending(rel => rel.DateAndTime)
                                 .FirstOrDefault();
        }
        public void DeletePersonRoomPairs(IEnumerable<PersonRoom> entities)
        {
            db.PersonRoom.RemoveRange(entities);
            //db.SaveChanges();
        }
        public void DeletePersonRoomPair(PersonRoom personRoom)
        {
            db.PersonRoom.Remove(personRoom);
        }
        public PersonRoom FindPersonRoomPair(int personId, int? roomId)
        {
            return db.PersonRoom.Where(pr => (pr.PersonId == personId) && (pr.RoomId == roomId)).FirstOrDefault();
        }
        public IEnumerable<PersonRoom> FindPersonRoomPairs(int roomId)
        {
            return db.PersonRoom.Where(pr => pr.RoomId == roomId);
        }
        public void AddRelocation(Relocation relocation)
        {
            db.Relocations.Add(relocation);
        }
        public IEnumerable<int> FindExistingRoomIds(List<int> roomsIds)
        {
            return db.Rooms.Where(r => roomsIds.Contains(r.Id)).Select(r => r.Id);
        }
        public IEnumerable<Person> FindExistingPeople(List<int> peopleIds)
        {
            return db.People.Where(p => peopleIds.Contains(p.Id));
        }
        public IEnumerable<PersonRoom> GetPersonRoomsAccess(int personId)
        {
            return db.PersonRoom.Where(pr => pr.PersonId == personId);
        }
        public IEnumerable<Room> GetRoomsOfBuilding(int id)
        {
            return db.Rooms.Include(r => r.Building).Where(r => r.BuildingId == id);
        }
        public IEnumerable<IGrouping<int, Relocation>> GetTodayRelocationsByPerson()
        {
            return db.Relocations.Include(rel => rel.Person)
                                  .Where(rel => (rel.DateAndTime.Date == DateTime.UtcNow.Date)&&rel.Success == true)
                                  .OrderByDescending(rel => rel.DateAndTime)
                                  .GroupBy(rel => rel.PersonId);
        }
        public IEnumerable<Relocation> GetTodayRelocationsOfPerson(int personId)
        {
            return db.Relocations.Include(rel => rel.Person)
                                  .Where(rel => (rel.DateAndTime.Date == DateTime.UtcNow.Date) && rel.Success == true && rel.PersonId == personId)
                                  .OrderByDescending(rel => rel.DateAndTime);
        }
        public IEnumerable<IGrouping<DateTime, Relocation>> GetRelocOfPersonByTimePeriod(int personId, DateTime startDate, DateTime endDate)
        {
            return db.Relocations.Include(rel => rel.Person)
                                  .Where(rel => rel.PersonId == personId && rel.Success == true)
                                  .Where(rel => (rel.DateAndTime >= startDate) && (rel.DateAndTime <= endDate))
                                  .OrderByDescending(rel => rel.DateAndTime)
                                  .GroupBy(rel => rel.DateAndTime.Date);
        }

        public IEnumerable<Door> GetDoorsOfRoom(int roomId)
        {
            return db.Doors.Where(rel => rel.FirstLocationId == roomId || rel.SecondLocationId == roomId);
        }
        public void DeleteDoors(IEnumerable<Door> doors)
        {
            db.Doors.RemoveRange(doors);
        }
        public IEnumerable<Room> GetNeighbourRooms(int roomId)
        {
            return db.Doors.Where(rel => rel.FirstLocationId == roomId)
                    .Select(rel => rel.SecondLocation)
                    .Union(db.Doors.Where(rel => rel.SecondLocationId == roomId).Select(rel => rel.FirstLocation));
        }
    }
}
