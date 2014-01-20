﻿using EquipmentGen.Core.Data.Items;
using EquipmentGen.Core.Generation.Factories;
using EquipmentGen.Core.Generation.Factories.Interfaces;
using EquipmentGen.Core.Generation.Generators;
using EquipmentGen.Core.Generation.Generators.Interfaces;
using EquipmentGen.Core.Generation.Providers.Interfaces;
using Moq;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Generation.Generators
{
    [TestFixture]
    public class MundaneItemGeneratorTests
    {
        private IPowerItemGenerator mundaneItemGenerator;
        private Mock<IPercentileResultProvider> mockPercentileResultProvider;
        private Mock<IAlchemicalItemGenerator> mockAlchemicalItemGenerator;
        private Mock<IGearGeneratorFactory> mockGearGeneratorFactory;
        private Mock<IToolGenerator> mockToolGenerator;

        [SetUp]
        public void Setup()
        {
            mockPercentileResultProvider = new Mock<IPercentileResultProvider>();
            mockPercentileResultProvider.Setup(p => p.GetPercentileResult("MundaneItems")).Returns(ItemsConstants.ItemTypes.AlchemicalItem);

            mockAlchemicalItemGenerator = new Mock<IAlchemicalItemGenerator>();
            mockGearGeneratorFactory = new Mock<IGearGeneratorFactory>();
            mockToolGenerator = new Mock<IToolGenerator>();

            mundaneItemGenerator = new MundaneItemGenerator(mockPercentileResultProvider.Object, mockAlchemicalItemGenerator.Object,
                mockGearGeneratorFactory.Object, mockToolGenerator.Object);
        }

        [Test]
        public void MundaneItemGeneratorProducesAlchemicalItems()
        {
            var item = new AlchemicalItem();
            mockAlchemicalItemGenerator.Setup(f => f.Generate()).Returns(item);
            mockPercentileResultProvider.Setup(p => p.GetPercentileResult("MundaneItems")).Returns(ItemsConstants.ItemTypes.AlchemicalItem);

            var result = mundaneItemGenerator.Generate();
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public void MundaneItemGeneratorProducesArmor()
        {
            var item = new Gear();
            var mockArmorGenerator = new Mock<IGearGenerator>();
            mockArmorGenerator.Setup(f => f.GenerateAtPower(ItemsConstants.Power.Mundane)).Returns(item);
            mockGearGeneratorFactory.Setup(f => f.CreateWith(ItemsConstants.ItemTypes.Armor)).Returns(mockArmorGenerator.Object);
            mockPercentileResultProvider.Setup(p => p.GetPercentileResult("MundaneItems")).Returns(ItemsConstants.ItemTypes.Armor);

            var result = mundaneItemGenerator.Generate();
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public void MundaneItemGeneratorProducesWeapons()
        {
            var item = new Gear();
            var mockWeaponGenerator = new Mock<IGearGenerator>();
            mockWeaponGenerator.Setup(f => f.GenerateAtPower(ItemsConstants.Power.Mundane)).Returns(item);
            mockGearGeneratorFactory.Setup(f => f.CreateWith(ItemsConstants.ItemTypes.Weapon)).Returns(mockWeaponGenerator.Object);
            mockPercentileResultProvider.Setup(p => p.GetPercentileResult("MundaneItems")).Returns(ItemsConstants.ItemTypes.Weapon);

            var result = mundaneItemGenerator.Generate();
            Assert.That(result, Is.EqualTo(item));
        }

        [Test]
        public void MundaneItemGeneratorProducesTools()
        {
            var item = new Tool();
            mockToolGenerator.Setup(f => f.Generate()).Returns(item);
            mockPercentileResultProvider.Setup(p => p.GetPercentileResult("MundaneItems")).Returns(ItemsConstants.ItemTypes.Tool);

            var result = mundaneItemGenerator.Generate();
            Assert.That(result, Is.EqualTo(item));
        }
    }
}