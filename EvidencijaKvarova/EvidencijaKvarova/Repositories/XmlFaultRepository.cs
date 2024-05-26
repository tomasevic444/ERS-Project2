using EvidencijaKvarova.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EvidencijaKvarova.Models;

namespace EvidencijaKvarova.Repositories
{
    public class XmlFaultRepository : IFaultRepository
    {
        private readonly string _filePath;

        public XmlFaultRepository(string filePath)
        {
            _filePath = filePath;
        }

        public void AddFault(Fault fault)
        {
            var faults = GetAllFaults();
            faults.Add(fault);
            SaveFaults(faults);
        }

        public Fault GetFaultById(string id)
        {
            return GetAllFaults().FirstOrDefault(f => f.Id == id);
        }

        public List<Fault> GetFaults(DateTime from, DateTime to)
        {
            return GetAllFaults().Where(f => f.CreationTime >= from && f.CreationTime <= to).ToList();
        }

        public void UpdateFault(Fault fault)
        {
            var faults = GetAllFaults();
            var existingFault = faults.FirstOrDefault(f => f.Id == fault.Id);
            if (existingFault != null)
            {
                faults.Remove(existingFault);
                faults.Add(fault);
                SaveFaults(faults);
            }
        }

        public List<Fault> GetAllFaults()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<Fault>();
                }

                var serializer = new XmlSerializer(typeof(List<Fault>));
                using (var stream = new FileStream(_filePath, FileMode.Open))
                {
                    return (List<Fault>)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, rethrow it, etc.)
                Console.WriteLine($"Failed to load faults: {ex.Message}");
                return new List<Fault>();
            }
        }

        private void SaveFaults(List<Fault> faults)
        {
            var serializer = new XmlSerializer(typeof(List<Fault>));
            using (var stream = new FileStream(_filePath, FileMode.Create))
            {
                serializer.Serialize(stream, faults);
            }
        }



    }
}
