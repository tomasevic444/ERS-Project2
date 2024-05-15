using EvidencijaKvarova.Interfaces;
using EvidencijaKvarova.Klase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EvidencijaKvarova.Repositories
{
    public class XmlElementRepository : IElementRepository
    {
        private readonly string _filePath;
        private List<ElectricalElement> _elements;

        public XmlElementRepository(string filePath)
        {
            _filePath = filePath;
            LoadElements();
        }

        public void AddElement(ElectricalElement element)
        {
            _elements.Add(element);
            SaveElements();
        }

        public ElectricalElement GetElementById(string id)
        {
            return _elements.FirstOrDefault(e => e.Id == id);
        }

        public List<ElectricalElement> GetAllElements()
        {
            return _elements;
        }

        private void LoadElements()
        {
            if (File.Exists(_filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ElectricalElement>));
                using (FileStream fs = new FileStream(_filePath, FileMode.Open))
                {
                    _elements = (List<ElectricalElement>)serializer.Deserialize(fs);
                }
            }
            else
            {
                _elements = new List<ElectricalElement>();
            }
        }

        private void SaveElements()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ElectricalElement>));
            using (FileStream fs = new FileStream(_filePath, FileMode.Create))
            {
                serializer.Serialize(fs, _elements);
            }
        }
    }
}
