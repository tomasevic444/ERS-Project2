using EvidencijaKvarova.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EvidencijaKvarova.Config
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly Configuration _configuration;

        public ConfigurationService(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException("Configuration file not found.", configFilePath);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            using (FileStream fs = new FileStream(configFilePath, FileMode.Open))
            {
                _configuration = (Configuration)serializer.Deserialize(fs);
            }
        }

        public double GetPriorityIncrementForRepair()
        {
            return _configuration.PriorityIncrementForRepair;
        }

        public double GetPriorityIncrementForTesting()
        {
            return _configuration.PriorityIncrementForTesting;
        }

        public double GetPriorityIncrementForHighVoltage()
        {
            return _configuration.PriorityIncrementForHighVoltage;
        }
    }

    [XmlRoot("Configuration")]
    public class Configuration
    {
        public double PriorityIncrementForRepair { get; set; }
        public double PriorityIncrementForTesting { get; set; }
        public double PriorityIncrementForHighVoltage { get; set; }
    }


}