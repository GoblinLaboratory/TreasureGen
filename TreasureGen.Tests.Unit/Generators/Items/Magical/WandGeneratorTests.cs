﻿using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class WandGeneratorTests
    {
        private MagicalItemGenerator wandGenerator;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private string power;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            wandGenerator = new WandGenerator(mockPercentileSelector.Object, mockChargesGenerator.Object);
            power = "power";
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void GenerateWand()
        {
            var wand = wandGenerator.GenerateAtPower(power);

            Assert.That(wand.Name, Does.StartWith("Wand of "));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
        }

        [Test]
        public void GetWandSpellFromSelector()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Wand);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("wand spell");

            var wand = wandGenerator.GenerateAtPower(power);
            Assert.That(wand.Name, Is.EqualTo("Wand of wand spell"));
        }

        [Test]
        public void GetChargesFromGenerator()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Wand);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("wand spell");
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "wand spell")).Returns(9266);

            var wand = wandGenerator.GenerateAtPower(power);
            Assert.That(wand.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateCustomWand()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var wand = wandGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(wand, template);
            Assert.That(wand.Name, Is.EqualTo(name));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
        }

        [Test]
        public void GenerateRandomCustomWand()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var wand = wandGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(wand, template);
            Assert.That(wand.Name, Is.EqualTo(name));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
        }

        [Test]
        public void GenerateFromSubset()
        {
            Assert.Fail();
        }

        [Test]
        public void GenerateDefaultFromSubset()
        {
            Assert.Fail();
        }

        [Test]
        public void GenerateFromEmptySubset()
        {
            Assert.That(() => generator.GenerateFromSubset(Enumerable.Empty<string>()), Throws.ArgumentException.With.Message.EqualTo("Cannot generate from an empty collection subset"));
        }
    }
}