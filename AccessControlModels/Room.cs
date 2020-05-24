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
        [Required]
        public string Name { get; set; }
        public double Area { get; set; }
        public string Description { get; set; }
        public RoomType Type { get; set; }        

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

        public override string ToString()
        {
            return this.Name;
        }
    }

    public enum RoomType
    {
        Ordinary,
        Hall
    }
}
