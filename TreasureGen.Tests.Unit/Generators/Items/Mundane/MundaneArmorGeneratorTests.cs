﻿using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Mundane;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Tests.Unit.Generators.Items.Mundane
{
    [TestFixture]
    public class MundaneArmorGeneratorTests
    {
        private MundaneItemGenerator mundaneArmorGenerator;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<ICollectionsSelector> mockAttributesSelector;
        private Mock<IBooleanPercentileSelector> mockBooleanPercentileSelector;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockAttributesSelector = new Mock<ICollectionsSelector>();
            mockBooleanPercentileSelector = new Mock<IBooleanPercentileSelector>();
            mundaneArmorGenerator = new MundaneArmorGenerator(mockPercentileSelector.Object, mockAttributesSelector.Object,
                mockBooleanPercentileSelector.Object);
            itemVerifier = new ItemVerifier();

            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneArmors)).Returns("armor type");
        }

        [Test]
        public void ReturnArmor()
        {
            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor, Is.Not.Null);
        }

        [Test]
        public void GetArmorTypeFromPercentileSelector()
        {
            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Name, Is.EqualTo("armor type"));
        }

        [Test]
        public void SetMasterworkTraitIfMasterwork()
        {
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(true);

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void DoNotSetMasterworkTraitIfNotMasterwork()
        {
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(false);

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Traits, Is.Not.Contains(TraitConstants.Masterwork));
        }

        [Test]
        public void GetShieldTypeIfResultIsShield()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneShields)).Returns("big shield");

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, "armor type")).Returns(attributes);

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateSizeFromPercentileSelector()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns("size");

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Traits, Contains.Item("size"));
            Assert.That(armor.Traits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAttributesForMundaneShields()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneShields)).Returns("big shield");
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns("size");

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = String.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, "big shield")).Returns(attributes);

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetSizesForMundaneShields()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneShields)).Returns("big shield");
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns("size");

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Traits, Contains.Item("size"));
        }

        [Test]
        public void GetMasterworkMundaneShield()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneShields)).Returns("big shield");
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(true);

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void DoNotGetMasterworkMundaneShield()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneShields)).Returns("big shield");
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(false);

            var armor = mundaneArmorGenerator.Generate();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Traits, Is.Not.Contains(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomMundaneArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns("size");
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(true);

            var armor = mundaneArmorGenerator.Generate(template);
            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Contains.Item("size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRandomCustomMundaneArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns("size");
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(true);

            var armor = mundaneArmorGenerator.Generate(template, true);
            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Contains.Item("size"));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void DoNotAddSizeToCustomArmorIfItAlreadyHasOne()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Add("size");

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var sizes = new[] { "size", "other size" };
            mockPercentileSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns(sizes);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes)).Returns("other size");
            mockBooleanPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork))
                .Returns(true);

            var armor = mundaneArmorGenerator.Generate(template);
            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Contains.Item("size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("other size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
        }
    }
}