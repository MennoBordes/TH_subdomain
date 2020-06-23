using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class KozijnStappenModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Step { get; set; }
        public string Description { get; set; }

        public enum Types
        {
            Raamkozijn = 1,
            Schuifpui = 2
        }
    }
}
