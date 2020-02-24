using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class Door
    {
        public int Id { get; set; }
        public Room FirstLocation { get; set; }
        public Room SecondLocation { get; set; }
        public bool MainExit { get; set; }
        public bool ExitToHall { get; set; }

    }
}
