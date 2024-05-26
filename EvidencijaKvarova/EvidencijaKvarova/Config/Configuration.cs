using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EvidencijaKvarova.Config
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        public double PriorityIncrementForRepair { get; set; }
        public double PriorityIncrementForTesting { get; set; }
        public double PriorityIncrementForHighVoltage { get; set; }
    }

}

