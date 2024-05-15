using EvidencijaKvarova.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidencijaKvarova.Interfaces
{
    public interface IElementRepository
    {
        void AddElement(ElectricalElement element);
        ElectricalElement GetElementById(string id);
        List<ElectricalElement> GetAllElements();
    }
}
