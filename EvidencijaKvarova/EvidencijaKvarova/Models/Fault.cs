using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Models
{
    public class Fault
    {
        public string Id { get; set; }
        public string ShortDescription { get; set; }
        public string DetailedDescription { get; set; }
        public string ElementId { get; set; }
        public DateTime CreationTime { get; set; }
        public string Status { get; set; }
        public List<EvidencijaKvarova.Models.Action> Actions { get; set; } = new List<EvidencijaKvarova.Models.Action>();
    }
}