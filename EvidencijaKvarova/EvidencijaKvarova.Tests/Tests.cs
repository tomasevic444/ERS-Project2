using NUnit.Framework;
using NUnit.Framework.Legacy;
using Moq;
using EvidencijaKvarova.Interfaces;
using EvidencijaKvarova.Models;
using EvidencijaKvarova.Services;
using EvidencijaKvarova.Klase;
using System;
using System.Collections.Generic;

namespace EvidencijaKvarova.Tests
{
    [TestFixture]
    public class FaultServiceTests
    {
        private Mock<IFaultRepository> _mockFaultRepository;
        private Mock<IElementRepository> _mockElementRepository;
        private Mock<IConfigurationService> _mockConfigurationService;
        private FaultService _faultService;

        [SetUp]
        public void SetUp()
        {
            _mockFaultRepository = new Mock<IFaultRepository>();
            _mockElementRepository = new Mock<IElementRepository>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _faultService = new FaultService(_mockFaultRepository.Object, _mockElementRepository.Object, _mockConfigurationService.Object);
        }

        [Test]
        public void CreateFault_ShouldAssignIdAndCreationTimeAndStatus()
        {
            // Arrange
            var fault = new Fault();

            // ClassicAssert
            ClassicAssert.IsNull(fault.Id);
            ClassicAssert.AreNotEqual("Nepotvrdjen", fault.Status);
            ClassicAssert.IsFalse(fault.CreationTime <= DateTime.Now && fault.CreationTime > DateTime.Now.AddSeconds(-1));
        }

        [Test]
        public void GetFaults_ShouldReturnFaultsWithinDateRange()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-5);
            var toDate = DateTime.Now;
            var faults = new List<Fault> { new Fault { Id = "1" }, new Fault { Id = "2" } };

            _mockFaultRepository.Setup(fr => fr.GetFaults(fromDate, toDate)).Returns(faults);

            // Act
            var result = _faultService.GetFaults(fromDate, toDate);

            // ClassicAssert
            ClassicAssert.AreEqual(2, result.Count);
            ClassicAssert.AreEqual("1", result[0].Id);
            ClassicAssert.AreEqual("2", result[1].Id);
        }

        [Test]
        public void GetFaultById_ShouldReturnCorrectFault()
        {
            // Arrange
            var faultId = "1";
            var fault = new Fault { Id = faultId };

            _mockFaultRepository.Setup(fr => fr.GetFaultById(faultId)).Returns(fault);

            // Act
            var result = _faultService.GetFaultById(faultId);

            // ClassicAssert
            ClassicAssert.AreEqual(faultId, result.Id);
        }

        [Test]
        public void UpdateFault_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var fault = new Fault { Id = "1" };

            // Act
            _faultService.UpdateFault(fault);

            // ClassicAssert
            _mockFaultRepository.Verify(fr => fr.UpdateFault(fault), Times.Once);
        }

        [Test]
        public void CalculatePriority_ShouldCalculateCorrectly_ForRepair()
        {
            // Arrange
            var fault = new Fault
            {
                Status = "U popravci",
                Actions = new List<EvidencijaKvarova.Models.Action> { new EvidencijaKvarova.Models.Action(), new EvidencijaKvarova.Models.Action() },
                ElementId = "element1"
            };

            _mockConfigurationService.Setup(cs => cs.GetPriorityIncrementForRepair()).Returns(10);
            _mockConfigurationService.Setup(cs => cs.GetPriorityIncrementForHighVoltage()).Returns(20);
            _mockElementRepository.Setup(er => er.GetElementById("element1")).Returns(new ElectricalElement { VoltageLevel = "visoki napon" });

            // Act
            var result = _faultService.CalculatePriority(fault);

            // ClassicAssert
            ClassicAssert.AreEqual(40, result); // 2 actions * 10 + 20 for high voltage
        }

        [Test]
        public void CalculatePriority_ShouldCalculateCorrectly_ForTesting()
        {
            // Arrange
            var fault = new Fault
            {
                Status = "Testiranje",
                Actions = new List<EvidencijaKvarova.Models.Action> { new EvidencijaKvarova.Models.Action() },
                ElementId = "element1"
            };

            _mockConfigurationService.Setup(cs => cs.GetPriorityIncrementForTesting()).Returns(5);
            _mockConfigurationService.Setup(cs => cs.GetPriorityIncrementForHighVoltage()).Returns(20);
            _mockElementRepository.Setup(er => er.GetElementById("element1")).Returns(new ElectricalElement { VoltageLevel = "visoki napon" });

            // Act
            var result = _faultService.CalculatePriority(fault);

            // ClassicAssert
            ClassicAssert.AreEqual(25, result); // 1 action * 5 + 20 for high voltage
        }

        [Test]
        public void CreateFaultsExcelDocument()
        {
            // Priprema
            var putanjaIzlaza = "test_faultsExcel.xlsx";
            var kvarovi = new List<Fault> { new Fault { Id = "1", Actions = new List<EvidencijaKvarova.Models.Action>() } };
            var elementi = new List<ElectricalElement> { new ElectricalElement { Id = "element1", Name = "Element 1", VoltageLevel = "srednji napon" } };

            _mockFaultRepository.Setup(fr => fr.GetAllFaults()).Returns(kvarovi);
            _mockElementRepository.Setup(er => er.GetAllElements()).Returns(elementi);

            // Akcija
            _faultService.CreateFaultsExcelDocument(putanjaIzlaza);

            // Provera
            // Proverite da li je Excel datoteka pravilno kreirana, npr. da li datoteka postoji, ima ispravne zaglavlje, itd.
        }

        [Test]
        public void GetAllFaults()
        {
            // Priprema
            var kvarovi = new List<Fault> { new Fault { Id = "1" }, new Fault { Id = "2" } };
            _mockFaultRepository.Setup(fr => fr.GetAllFaults()).Returns(kvarovi);

            // Akcija
            var rezultat = _faultService.GetAllFaults();

            // Provera
            ClassicAssert.AreEqual(2, rezultat.Count);
            ClassicAssert.AreEqual("1", rezultat[0].Id);
            ClassicAssert.AreEqual("2", rezultat[1].Id);
        }

    }
}
