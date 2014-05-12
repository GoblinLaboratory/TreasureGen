﻿using System;
using D20Dice;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using EquipmentGen.Generators.Interfaces.Items.Mundane;
using EquipmentGen.Generators.Items.Magical;
using EquipmentGen.Generators.RuntimeFactories;
using EquipmentGen.Generators.RuntimeFactories.Interfaces;
using EquipmentGen.Selectors.Interfaces;
using Moq;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Generators.RuntimeFactories
{
    [TestFixture]
    public class MagicalItemGeneratorFactoryTests
    {
        private IMagicalItemGeneratorFactory factory;
        private Mock<IIntelligenceGenerator> mockIntelligenceGenerator;

        [SetUp]
        public void Setup()
        {
            var mockPercentileSelector = new Mock<IPercentileSelector>();
            var mockTraitsGenerator = new Mock<IMagicalItemTraitsGenerator>();
            mockIntelligenceGenerator = new Mock<IIntelligenceGenerator>();
            var mockAttributesSelector = new Mock<IAttributesSelector>();
            var mockChargesGenerator = new Mock<IChargesGenerator>();
            var mockDice = new Mock<IDice>();
            var mockSpellGenerator = new Mock<ISpellGenerator>();
            var mockCurseGenerator = new Mock<ICurseGenerator>();
            var mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            var mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            var mockMaterialsGenerator = new Mock<ISpecialMaterialGenerator>();
            var mockMagicItemTraitsGenerator = new Mock<IMagicalItemTraitsGenerator>();
            var mockSpecificGearGenerator = new Mock<ISpecificGearGenerator>();

            factory = new MagicalItemGeneratorFactory(mockPercentileSelector.Object, mockTraitsGenerator.Object, mockIntelligenceGenerator.Object,
                mockAttributesSelector.Object, mockSpecialAbilitiesGenerator.Object, mockMaterialsGenerator.Object, mockMagicItemTraitsGenerator.Object,
                mockChargesGenerator.Object, mockDice.Object, mockSpellGenerator.Object, mockCurseGenerator.Object, mockTypeAndAmountPercentileSelector.Object,
                mockSpecificGearGenerator.Object);
        }

        [Test]
        public void CreatePotionGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Potion);
            Assert.That(generator, Is.TypeOf<PotionGenerator>());
        }

        [Test]
        public void CreateRingGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Ring);
            Assert.That(generator, Is.TypeOf<RingGenerator>());
        }

        [Test]
        public void CreateRodGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Rod);
            Assert.That(generator, Is.TypeOf<RodGenerator>());
        }

        [Test]
        public void CreateScrollGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Scroll);
            Assert.That(generator, Is.TypeOf<ScrollGenerator>());
        }

        [Test]
        public void CreateStaffGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Staff);
            Assert.That(generator, Is.TypeOf<StaffGenerator>());
        }

        [Test]
        public void CreateWandGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Wand);
            Assert.That(generator, Is.TypeOf<WandGenerator>());
        }

        [Test]
        public void CreateWondrousItemGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.WondrousItem);
            Assert.That(generator, Is.TypeOf<WondrousItemGenerator>());
        }

        [Test]
        public void CreateArmorGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Armor);
            Assert.That(generator, Is.TypeOf<MagicalArmorGenerator>());
        }

        [Test]
        public void CreateWeaponGenerator()
        {
            var generator = factory.CreateGeneratorOf(ItemTypeConstants.Weapon);
            Assert.That(generator, Is.TypeOf<MagicalWeaponGenerator>());
        }

        [Test]
        public void InvalidTypeThrowsException()
        {
            Assert.That(() => factory.CreateGeneratorOf("invalid type"), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}