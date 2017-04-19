﻿using Moq;
using NUnit.Framework;
using RollGen;
using System;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class ScrollGeneratorTests
    {
        private MagicalItemGenerator scrollGenerator;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private Mock<Dice> mockDice;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockDice = new Mock<Dice>();
            scrollGenerator = new ScrollGenerator(mockDice.Object, mockSpellGenerator.Object);

            itemVerifier = new ItemVerifier();

            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(1);
        }

        [Test]
        public void GenerateScroll()
        {
            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Minor);
            Assert.That(scroll.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.Name, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.IsMagical, Is.True);
            Assert.That(scroll.Attributes.Single(), Is.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Scroll));
        }

        [Test]
        public void GetTypeAndLevelFromSpellGenerator()
        {
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(9266);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 9266)).Returns("spell");

            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Minor);
            Assert.That(scroll.Name, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.Traits, Contains.Item("spell type"));
            Assert.That(scroll.Contents, Contains.Item("spell (9266)"));
        }

        [Test]
        public void MinorScrollsHave1d3Spells()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(9266);

            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Minor);
            Assert.That(scroll.Contents.Count, Is.EqualTo(9266));
        }

        [Test]
        public void MediumScrollsHave1d4Spells()
        {
            mockDice.Setup(d => d.Roll(1).d(4).AsSum()).Returns(9266);

            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Medium);
            Assert.That(scroll.Contents.Count, Is.EqualTo(9266));
        }

        [Test]
        public void MajorScrollsHave1d6Spells()
        {
            mockDice.Setup(d => d.Roll(1).d(6).AsSum()).Returns(9266);

            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Major);
            Assert.That(scroll.Contents.Count, Is.EqualTo(9266));
        }

        [Test]
        public void UnknownPowerThrowsError()
        {
            Assert.That(() => scrollGenerator.GenerateAtPower("power"), Throws.ArgumentException);
        }

        [Test]
        public void GenerateSpellTypeOnce()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(9266);

            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Minor);
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Once);
        }

        [Test]
        public void GenerateLevelAndSpellEachTime()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(9266);

            var scroll = scrollGenerator.GenerateAtPower(PowerConstants.Minor);
            mockSpellGenerator.Verify(g => g.GenerateLevel(PowerConstants.Minor), Times.Exactly(9266));
            mockSpellGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(9266));
        }

        [Test]
        public void GenerateCustomScroll()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var scroll = scrollGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(scroll, template);
            Assert.That(scroll.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.IsMagical, Is.True);
            Assert.That(scroll.Attributes.Single(), Is.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.Magic.Bonus, Is.EqualTo(0));
            Assert.That(scroll.Magic.Charges, Is.EqualTo(0));
            Assert.That(scroll.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Scroll));
        }

        [Test]
        public void GenerateRandomCustomScroll()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var scroll = scrollGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(scroll, template);
            Assert.That(scroll.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.IsMagical, Is.True);
            Assert.That(scroll.Attributes.Single(), Is.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.Magic.Bonus, Is.EqualTo(0));
            Assert.That(scroll.Magic.Charges, Is.EqualTo(0));
            Assert.That(scroll.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Scroll));
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