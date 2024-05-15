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
        private readonly Dictionary<string, double> _configurations;

        public ConfigurationService(string configFilePath)
        {
            _configurations = LoadConfigurations(configFilePath);
        }

        public double GetPriorityIncrementForRepair()
        {
            return _configurations["RepairIncrement"];
        }

        public double GetPriorityIncrementForTesting()
        {
            return _configurations["TestingIncrement"];
        }

        public double GetPriorityIncrementForHighVoltage()
        {
            return _configurations["HighVoltageIncrement"];
        }

        private Dictionary<string, double> LoadConfigurations(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // Default values if configuration file does not exist
                return new Dictionary<string, double>
            {
                { "RepairIncrement", 1.0 },
                { "TestingIncrement", 0.5 },
                { "HighVoltageIncrement", 1.0 }
            };
            }

            var serializer = new XmlSerializer(typeof(List<Configuration>));
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var configurations = (List<Configuration>)serializer.Deserialize(stream);
                return configurations.ToDictionary(c => c.Key, c => c.Value);
            }
        }

        [XmlRoot("Configuration")]
        public class Configuration
        {
            [XmlElement("Key")]
            public string Key { get; set; }
            [XmlElement("Value")]
            public double Value { get; set; }
        }
    }
}
