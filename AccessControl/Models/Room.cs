using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Number { get; set; }
        public RoomType Type { get; set; }
        //public bool MainEntrance { get; set; }
    }

    public enum RoomType
    {
        Ordinary,
        Hall
    }
}
