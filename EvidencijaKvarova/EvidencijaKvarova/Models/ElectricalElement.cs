using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Klase
{
    public class ElectricalElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string VoltageLevel { get; set; } = "srednji napon";
    }
}
