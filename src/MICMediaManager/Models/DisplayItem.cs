using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MICMediaManager.Models
{
    public class DisplayItem
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public string ImageUri { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}
