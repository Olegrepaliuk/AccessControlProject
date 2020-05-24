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

        public IEnumerable<Reader> Readers
        {
            get
            {
                return db.Readers;
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

        public void PutReader(Reader reader)
        {
            db.Entry(reader).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Person GetPersonById(int id)
        {
            return db.People.Find(id);
        }

        public Person GetPersonByCardNum(string cardKey)
        {
            return db.People.FirstOrDefault(p => p.CardKey == cardKey);
        }

        public void AddPerson(Person person)
        {
            db.People.Add(person);
            db.SaveChanges();
        }

        public string DeletePerson(int id)
        {
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
            return db.Rooms.Find(id);
        }


        public int CountPeople()
        {
            return db.People.Count();
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
 
        public void PutBuilding(Building building)
        {
            db.Entry(building).State = EntityState.Modified;
            db.SaveChanges();
        }


        public void AddDoor(Door door)
        {
            db.Doors.Add(door);
        }

        public void DeleteDoor(Door door)
        {
            db.Doors.Remove(door);
        }
        public void DeleteDoors(IEnumerable<Door> doors)
        {
            db.Doors.RemoveRange(doors);
        }
        public void AddPersonRoom(PersonRoom pr)
        {
            db.PersonRoom.Add(pr);
        }

        public void AddReader(Reader reader)
        {
            db.Readers.Add(reader);
        }
        public void DeleteReader(Reader reader)
        {
            db.Readers.Remove(reader);
        }
        public Reader GetReaderById(int readerId)
        {
            return db.Readers.Include(r => r.CurrentLoc).Include(r => r.NextLoc).FirstOrDefault(r => r.Id == readerId);
        }
        public IEnumerable<Reader> GetAllReaders()
        {
            return db.Readers.Include(r => r.NextLoc).Include(r => r.CurrentLoc);
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

        public Door GetDoorOfRooms(int? firstRoomId, int? secondRoomId)
        {
            return db.Doors
                    .Where(d => d.FirstLocationId == firstRoomId && d.SecondLocationId == secondRoomId)
                    .Union(db.Doors.Where(d => d.SecondLocationId == firstRoomId && d.FirstLocationId == secondRoomId)).FirstOrDefault();
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
        public IEnumerable<Room> GetNeighbourRooms(int roomId)
        {
            return db.Doors.Where(rel => rel.FirstLocationId == roomId)
                    .Select(rel => rel.SecondLocation)
                    .Union(db.Doors.Where(rel => rel.SecondLocationId == roomId).Select(rel => rel.FirstLocation));
        }

        public IEnumerable<Relocation> GetAllRelocations()
        {
            return db.Relocations
                .Include(rel => rel.FromLoc)
                .Include(rel => rel.ToLoc)
                .Include(rel => rel.Person)
                .OrderByDescending(rel => rel.DateAndTime);
        }
    }
}
