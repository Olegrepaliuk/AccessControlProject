using AccessControl.Models;
using AccessControlModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class ControlService
    {
        private ControlRepository repo;
        public ControlService(ControlRepository repository)
        {
            repo = repository;
        }
        #region Person
        public List<Person> GetAllPeople()
        {
            return repo.People.ToList();
        }
        public Person GetPersonById(int id)
        {
            return repo.GetPersonById(id);
        }

        public void CreatePerson(Person person)
        {
            repo.AddPerson(person);
        }

        public void UpdatePerson(Person person)
        {
            repo.PutPerson(person);
        }
        public string DeletePerson(int id)
        {
            var person = repo.GetPersonById(id);
            if (person != null)
            {
                var personRoomPairs = repo.FindPersonRoomPairsByPersonId(id);
                if (personRoomPairs.Count() > 0) repo.DeletePersonRoomPairs(personRoomPairs);
                repo.DeletePerson(person);
                repo.SaveChanges();
                return "deleted";
            }
            else
            {
                return "NotFound";
            }
        }

        public int CountAllPeople()
        {
            return repo.CountPeople();
        }

        #endregion

        #region Room
        public List<Room> GetAllRooms()
        {
            return repo.Rooms.ToList();
        }
        public Room GetRoomById(int id)
        {
            return repo.GetRoomById(id);
        }

        public bool TryMoveToOtherLoc(string cardKey, int readerId)
        {
            bool access = true;
            var reader = repo.GetReaderById(readerId);
            if (reader == null) return false;
            var person = repo.GetPersonByCardNum(cardKey);
            if (person == null) return false;
            if(reader.NextLocId != null)
            {
                var entity = repo.FindPersonRoomPair(person.Id, reader.NextLocId);
                if (entity == null)
                {
                    access = false;
                }
            }
            DateTime dateTime = DateTime.UtcNow;
            var relocation = new Relocation
            {
                PersonId = person.Id,
                FromLocId = reader.CurrentLocId,
                ToLocId = reader.NextLocId,
                DateAndTime = dateTime,
                Success = access
            };
            repo.AddRelocation(relocation);
            repo.SaveChanges();
            return access;
        }

        public void CreateRoom(Room room)
        {
            repo.AddRoom(room);
        }
        public void UpdateRoom(Room room)
        {
            repo.PutRoom(room);
        }
        public string DeleteRoom(int id)
        {
            var room = repo.GetRoomById(id);
            if(room != null)
            {
                var personRoomPairs = repo.FindPersonRoomPairsByRoomId(id);
                if (personRoomPairs.Count() > 0) repo.DeletePersonRoomPairs(personRoomPairs);
                var connectedReaders = repo.GetAllReadersByRoomId(id);
                if (connectedReaders.Count() > 0) repo.DeleteReaderEntities(connectedReaders);
                repo.DeleteRoom(room);                           
                repo.SaveChanges();
                return "deleted";
            }
            return "NotFound";
        }
        #endregion
        #region Reader
        public void CreateReader(Reader reader)
        {
            repo.AddReader(reader);
            repo.SaveChanges();
        }
        public void UpdateReader(Reader reader)
        {
            repo.PutReader(reader);
        }
        public string DeleteReader(int id)
        {
            var reader = repo.GetReaderById(id);
            if(reader != null)
            {
                repo.DeleteReader(reader);
                return "deleted";
            }
            return "NotFound";
        }

        public IEnumerable<Reader> GetAllReaders()
        {
            return repo.GetAllReaders();
        }
        public Reader GetReaderById(int id)
        {
            return repo.GetReaderById(id);
        }
        #endregion
        #region Building
        public List<Building> GetAllBuildings()
        {
            throw new NotImplementedException();
        }
        public Building GetBuildingById(int id)
        {
            throw new NotImplementedException();
        }
        public List<Room> GetRoomsOfBuilding(int id)
        {
            throw new NotImplementedException();
        }

        public void CreateBuilding(Building building)
        {
            throw new NotImplementedException();
        }
        public void UpdateBuilding(Building building)
        {
            throw new NotImplementedException();
        }
        #endregion
        public void ConnectRooms(int id, int? connectedRoomId)
        {
            AddDoor(id, connectedRoomId);
        }


        public void UpdatePersonAccess(int id, IEnumerable<int> roomsId)
        {
            var setIds = roomsId.ToHashSet();
            var pairs = repo.GetPersonRoomsAccess(id);
            foreach (var item in pairs)
            {
                if (!setIds.Contains(item.RoomId))
                {
                    var extraEntity = repo.FindPersonRoomPair(item.PersonId, item.RoomId);
                    if (extraEntity != null)
                    {
                        repo.DeletePersonRoomPair(extraEntity);
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
                repo.AddPersonRoom(personRoom);
            }

            repo.SaveChanges();
        }

        public void UpdateRoomConnections(int id, IEnumerable<int> roomsId)
        {
            var setIds = roomsId.ToHashSet();
            var connected = repo.GetNeighbourRooms(id);
            foreach (var item in connected)
            {
                if(item != null)
                {           
                    if (!setIds.Contains(item.Id))
                    {
                        if (item.Type == RoomType.Hall) throw new Exception("Unable to disconnect room from hall");
                        var finder = new WayFinder(repo, item.Id);
                        Room extraEntity = null; //= repo.FindRoom(item.Id);

                        if (extraEntity != null)
                        {
                            var door = repo.GetDoorOfRooms(item.Id, id);
                            if (door != null) repo.DeleteDoor(door);
                        }

                    }
                    else
                    {
                        setIds.Remove(item.Id);
                    }
                }
                else
                {
                    if(!setIds.Contains(-1))
                    {
                        var extraEntity = repo.GetDoorOfRooms(id, null);
                    }
                    else
                    {
                        setIds.Remove(-1);
                    }
                }


            }

            foreach (var roomIdItem in setIds)
            {
                var personRoom = new PersonRoom(id, roomIdItem);
                repo.AddPersonRoom(personRoom);
            }

            repo.SaveChanges();
        }

        public bool TryDeleteRoom(int id)
        {
            /*
            WayFinder finder = new WayFinder(repo, id);
            if (!finder.DeleteAbility()) return false;
            var peopleConnected = repo.FindPersonRoomPairsByPersonId(id);
            if (peopleConnected.Count() > 0) repo.DeletePersonRoomPairs(peopleConnected);
            var dooorsWithConnectedRooms = repo.GetDoorsOfRoom(id);
            if (dooorsWithConnectedRooms.Count() > 0) repo.DeleteDoors(dooorsWithConnectedRooms);
            repo.DeleteRoom(id);
            */
            return true;
        }

        public IEnumerable<Room> GetConnectedRooms(int roomId)
        {
            return repo.GetNeighbourRooms(roomId);
        }

        public IEnumerable<Room> GetRoomsOfPersonAccess(int personId)
        {
            List<Room> resultRooms = new List<Room>();
            var personRooms = repo.GetPersonRoomsAccess(personId);
            foreach (var item in personRooms)
            {
                var room = repo.GetRoomById(item.RoomId);
                if (room != null) resultRooms.Add(room);
            }
            return resultRooms;
        }



        public void ConnectRoomWithOthers(int roomId, List<int> otherRoomsIds)
        {
            Door door;
            if (otherRoomsIds.Contains(-1))
            {
                door = new Door(roomId, null);
                repo.AddDoor(door);
            }
            var existingIds = repo.FindExistingRoomIds(otherRoomsIds);
            foreach (var item in existingIds)
            {
                door = new Door(roomId, item);
                repo.AddDoor(door);
            }
            repo.SaveChanges();
        }



        public int? FindLastLoggedPersonLocId(int personId)
        {
            var lastReloc = repo.FindLastPersonRelocation(personId);
            if (lastReloc == null)
            {
                return -1;
            }
            return lastReloc.ToLocId;

        }

        public void AddDoor(int? firstLocId, int? secLocId)
        {
            var door = new Door(firstLocId, secLocId);
            repo.AddDoor(door);
            repo.SaveChanges();
        }

        public bool TryMoveToOtherLoc(int personId, int? fromLocId, int? toLocId)
        {
            bool access;
            var entity = repo.FindPersonRoomPair(personId, toLocId);
            if(entity == null)
            {
                access = false;
            }
            else
            {
                access = true;
            }
            DateTime dateTime = DateTime.UtcNow;
            var relocation = new Relocation
            {
                PersonId = personId,
                FromLocId = fromLocId,
                ToLocId = toLocId,
                DateAndTime = dateTime,
                Success = access
            };
            repo.AddRelocation(relocation);
            repo.SaveChanges();
            return access;
        }
 
        public List<int> GetPeopleIdsInsideNow()
        {
            var peopleIds = new List<int>();
            var relocByPerson = repo.GetTodayRelocationsByPerson();
            foreach (var group in relocByPerson)
            {
                if (group.Count() > 0 && group.First().ToLocId != null)
                {
                    peopleIds.Add(group.Key);
                }
            }
            return peopleIds;
        }
        public List<Person> GetPeopleInsideNow()
        {
            var peopleIds = GetPeopleIdsInsideNow();
            return repo.FindExistingPeople(peopleIds).ToList();
        }


        public int CountPeopleInsideNow()
        {
            return GetPeopleInsideNow().Count;
        }

        public bool CheckIfPersonInsideNow(int personId)
        {
            var relocations = repo.GetTodayRelocationsOfPerson(personId);
            if(relocations.Count() > 0)
            {
                if (relocations.First().ToLoc != null) return true;
            }
            return false;
        }

        public int CountDaysWithVisiting(int personId, int daysAmt)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-daysAmt);
            var relocations = repo.GetRelocOfPersonByTimePeriod(personId, startDate, endDate);
            return relocations.Count();
        }
        public int CountSuccessEntersToday()
        {
            return repo.CountSuccessEntersToday();
        }
        public int CountFailedEntersToday()
        {
            return repo.CountFailedEntersToday();
        }
        public IEnumerable<Relocation> GetAllRelocations()
        {
            return repo.GetAllRelocations();
        }
        public void GenerateData()
        {
            var people = new Person[]
            {
                new Person{Name = "Ivan Prokopenko", CardKey = "ewr1", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Ihor Karavaev", CardKey = "ewr2", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Potap Pavlenko", CardKey = "ewr3", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Anna Melnyk", CardKey = "ewr4", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Serhii Kyrylenko", CardKey = "ewr5", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Anton Shevchenko", CardKey = "ewr6", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Ivan Bondarenko", CardKey = "ewr7", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Petro Koval", CardKey = "ewr8", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Pavlo Shevchenko", CardKey = "ewr9", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Ksenia Tkachuk", CardKey = "ewr10", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Viktor Moroz", CardKey = "ewr11", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Zlata Rudenko", CardKey = "ewr12", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Andrii Prokopenko", CardKey = "ewr13", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Oleksii Prokopenko", CardKey = "ewr14", CardValidTil=DateTime.Parse("24.03.2021")},
                new Person{Name = "Viktoria Tkach", CardKey = "ewr15", CardValidTil=DateTime.Parse("24.03.2021")}
            };
            repo.AddPeople(people);

            var rooms = new Room[]
            {
                new Room{Name = "Main hall", Type = RoomType.Hall, Area = 500},
                new Room{Name = "Room 2", Type = RoomType.Ordinary, Area = 134},
                new Room{Name = "Room 3", Type = RoomType.Ordinary, Area = 138},
                new Room{Name = "Room 4", Type = RoomType.Ordinary, Area = 114},
                new Room{Name = "Room 5", Type = RoomType.Ordinary, Area = 185},
                new Room{Name = "Room 6", Type = RoomType.Ordinary, Area = 147},
                new Room{Name = "Kitchen", Type = RoomType.Ordinary, Area = 192},
                new Room{Name = "Stockroom", Type = RoomType.Ordinary, Area = 212},
                new Room{Name = "Security room", Type = RoomType.Ordinary, Area = 115},
                new Room{Name = "Server room", Type = RoomType.Ordinary, Area = 121}
            };
            repo.AddRooms(rooms);

            var readers = new Reader[]
            {
                new Reader{CurrentLoc = rooms[0], NextLoc = rooms[1]},
                new Reader{CurrentLoc = rooms[0], NextLoc = rooms[2]},
                new Reader{CurrentLoc = rooms[0], NextLoc = rooms[4]},
                new Reader{CurrentLocId = null, NextLoc = rooms[0]},
                new Reader{CurrentLoc = rooms[0], NextLoc = rooms[3]},
                new Reader{CurrentLoc = rooms[1], NextLoc = rooms[6]},
                new Reader{CurrentLoc = rooms[0], NextLoc = rooms[5]},
                new Reader{CurrentLoc = rooms[5], NextLoc = rooms[7]},
                new Reader{CurrentLoc = rooms[7], NextLoc = rooms[9]},
                new Reader{CurrentLoc = rooms[7], NextLoc = rooms[8]},

                new Reader{CurrentLoc = rooms[1], NextLoc = rooms[0]},
                new Reader{CurrentLoc = rooms[2], NextLoc = rooms[0]},
                new Reader{CurrentLoc = rooms[4], NextLoc = rooms[0]},
                new Reader{CurrentLoc = rooms[0], NextLocId = null},
                new Reader{CurrentLoc = rooms[3], NextLoc = rooms[0]},
                new Reader{CurrentLoc = rooms[6], NextLoc = rooms[1]},
                new Reader{CurrentLoc = rooms[5], NextLoc = rooms[0]},
                new Reader{CurrentLoc = rooms[7], NextLoc = rooms[5]},
                new Reader{CurrentLoc = rooms[9], NextLoc = rooms[7]},
                new Reader{CurrentLoc = rooms[8], NextLoc = rooms[7]}

            };
            repo.AddReaders(readers);
            var personRoomPairs = new List<PersonRoom>();
            foreach(var person in people)
            {
                for(int i = 0; i<7;i++)
                {
                    var pair = new PersonRoom(person.Id, rooms[i].Id);
                    personRoomPairs.Add(pair);
                }
            }
            personRoomPairs.Add(new PersonRoom(1, rooms[7].Id));
            personRoomPairs.Add(new PersonRoom(2, rooms[7].Id));
            personRoomPairs.Add(new PersonRoom(3, rooms[7].Id));
            personRoomPairs.Add(new PersonRoom(1, rooms[8].Id));
            personRoomPairs.Add(new PersonRoom(2, rooms[8].Id));
            personRoomPairs.Add(new PersonRoom(1, rooms[9].Id));
            repo.AddPersonRoomPairs(personRoomPairs);

            var relocations = new List<Relocation>();
            var rand = new Random();
            
            
            foreach (var person in people)
            {
                var currentDate = DateTime.Now;
                currentDate = currentDate.AddDays(-2);
                int randMin;
                int randSec;
                for (int i = 0; i < 3; i++)
                {
                    string stringDate = currentDate.ToString("d");
                    randMin = rand.Next(1, 59);
                    randSec = rand.Next(1, 59);
                    relocations.Add(
                        new Relocation
                        {
                            FromLocId = null,
                            ToLocId = rooms[0].Id,
                            PersonId = person.Id,
                            Success = true,
                            DateAndTime = DateTime.Parse($"{stringDate} 10:{randMin}:{randSec}")
                        });
                    int secondRoomId = 0;
                    if (person.Id % 3 == 0) secondRoomId = rooms[1].Id;
                    if (person.Id % 3 == 1) secondRoomId = rooms[2].Id;
                    if (person.Id % 3 == 2) secondRoomId = rooms[4].Id;
                    randMin = rand.Next(1, 59);
                    randSec = rand.Next(1, 59);
                    relocations.Add(
                        new Relocation
                        {
                            FromLocId = rooms[0].Id,
                            ToLocId = secondRoomId,
                            PersonId = person.Id,
                            Success = true,
                            DateAndTime = DateTime.Parse($"{stringDate} 11:{randMin}:{randSec}")
                        });

                    randMin = rand.Next(1, 59);
                    randSec = rand.Next(1, 59);
                    relocations.Add(
                        new Relocation
                        {
                            FromLocId = secondRoomId,
                            ToLocId = rooms[0].Id,
                            PersonId = person.Id,
                            Success = true,
                            DateAndTime = DateTime.Parse($"{stringDate} 12:{randMin}:{randSec}")
                        });
                    randMin = rand.Next(1, 59);
                    randSec = rand.Next(1, 59);
                    relocations.Add(
                        new Relocation
                        {
                            FromLocId = rooms[0].Id,
                            ToLocId = null,
                            PersonId = person.Id,
                            Success = true,
                            DateAndTime = DateTime.Parse($"{stringDate} 13:{randMin}:{randSec}")
                        });
                    currentDate = currentDate.AddDays(1);
                }
               
            }
            repo.AddRelocations(relocations);
        }
    }
}
