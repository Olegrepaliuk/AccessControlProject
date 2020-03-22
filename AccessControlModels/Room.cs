using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlModels
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Number { get; set; }
        public RoomType Type { get; set; }
        
        public Building Building { get; set; }
        public int? BuildingId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Room))
            {
                return false;
            }
            return (this.Id == ((Room)obj).Id);

        }
    }



    public enum RoomType
    {
        Ordinary,
        Hall
    }
}
