using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductionSystem.Tests
{
    [TestFixture]
    public class FactoryTests
    {
        private Warehouse _warehouse;
        private Factory _factory;

        [SetUp]
        public void Setup()
        {
            _warehouse = new Warehouse(1000);
            _factory = new Factory("A", 50, _warehouse);
        }

        [Test]
        public void StartProduction_ShouldAddProductsToWarehouse()
        {
            // Arrange
            var initialLoad = _warehouse.CurrentLoad;

            // Act
            _factory.StartProduction();
            Thread.Sleep(2000); // Достаточно времени для производства

            // Assert
            Assert.Greater(_warehouse.CurrentLoad, initialLoad);
        }
    }

    [TestFixture]
    public class WarehouseTests
    {
        private Warehouse _warehouse;

        [SetUp]
        public void Setup()
        {
            _warehouse = new Warehouse(1000);
        }

        [Test]
        public void AddProduct_ShouldIncreaseCurrentLoad()
        {
            // Arrange
            var product = new Product("A", 1, "Standard");
            var quantity = 50;

            // Act
            _warehouse.AddProduct(product, quantity);

            // Assert
            Assert.AreEqual(quantity, _warehouse.CurrentLoad);
        }

        [Test]
        public void LoadTruck_ShouldDecreaseCurrentLoad()
        {
            // Arrange
            var product = new Product("A", 1, "Standard");
            var quantity = 50;
            _warehouse.AddProduct(product, quantity);

            // Act
            var load = _warehouse.LoadTruck(30);

            // Assert
            Assert.AreEqual(20, _warehouse.CurrentLoad);
            Assert.AreEqual(30, load["A"]);
        }
    }

    [TestFixture]
    public class TruckTests
    {
        private Warehouse _warehouse;
        private Truck _truck;

        [SetUp]
        public void Setup()
        {
            _warehouse = new Warehouse(1000);
            _truck = new Truck(100);
        }

        [Test]
        public void StartDelivery_ShouldUpdateTruckStatistics()
        {
            // Arrange
            var product = new Product("A", 1, "Standard");
            _warehouse.AddProduct(product, 100);

            // Act
            _truck.StartDelivery(_warehouse);
            Thread.Sleep(6000); // Достаточно времени для доставки

            // Assert
            Assert.AreEqual(100, _truck.AverageLoad);
            Assert.AreEqual("A: 100", _truck.LoadComposition);
        }
    }
}