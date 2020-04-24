﻿using AccessControl.Models;
using AccessControlModels;
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
            return repo.DeletePerson(id);
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
            return repo.DeleteRoom(id);
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

        public bool TryDeleteRoom(int id)
        {
            List<Room> neighbourRooms = repo.GetNeighbourRooms(id).ToList();
            WayFinder finder = new WayFinder(repo);
            finder.RoomIdToDelete = id;
            foreach (var item in neighbourRooms)
            {
                if (!finder.DeleteAbility(item.Id)) return false;
            }
            repo.DeleteRoom(id);
            return true;
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
        
        public List<Person> GetPeopleInsideNow()
        {
            var peopleIds = new List<int>();
            var relocByPerson = repo.GetTodayRelocationsByPerson();
            foreach (var group in relocByPerson)
            {
                if(group.Count()>0&&group.First().ToLoc != null)
                {
                    peopleIds.Add(group.Key);
                }
            }
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

    }
}
