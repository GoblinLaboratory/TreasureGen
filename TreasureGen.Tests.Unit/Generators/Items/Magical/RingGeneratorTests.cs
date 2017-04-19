﻿using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class RingGeneratorTests
    {
        private MagicalItemGenerator ringGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<ICollectionsSelector> mockAttributesSelector;
        private Mock<IMagicalItemTraitsGenerator> mockTraitsGenerator;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private TypeAndAmountPercentileResult result;
        private string power;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockAttributesSelector = new Mock<ICollectionsSelector>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockTraitsGenerator = new Mock<IMagicalItemTraitsGenerator>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            result = new TypeAndAmountPercentileResult();
            ringGenerator = new RingGenerator(mockPercentileSelector.Object, mockAttributesSelector.Object, mockSpellGenerator.Object, mockChargesGenerator.Object, mockTypeAndAmountPercentileSelector.Object);
            power = "power";
            itemVerifier = new ItemVerifier();

            result.Amount = 9266;
            result.Type = "ring of ability";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(result);

        }

        [Test]
        public void GenerateRing()
        {
            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo("ring of ability"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring of ability"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, result.Type)).Returns(attributes);

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetChargesIfCharged()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, result.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, result.Type)).Returns(9266);

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void DoNotGetChargesIfNotCharged()
        {
            var attributes = new[] { "new attribute" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, result.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, result.Type)).Returns(9266);

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void MinorSpellStoringHasSpell()
        {
            result.Type = RingConstants.SpellStoring_Minor;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(2);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 2)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Minor));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 2)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void MinorSpellStoringHasAtMost3SpellLevels()
        {
            result.Type = RingConstants.SpellStoring_Minor;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2").Returns("spell 3").Returns("spell 4");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Minor));
            Assert.That(ring.Contents, Contains.Item("spell 1 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 2 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 3 (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(3));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(3));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.AtLeast(3));
        }

        [Test]
        public void MinorSpellStoringAllowsDuplicateSpells()
        {
            result.Type = RingConstants.SpellStoring_Minor;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Minor));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(3));
            Assert.That(ring.Contents.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void SpellStoringHasSpell()
        {
            result.Type = RingConstants.SpellStoring;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(3);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 3)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 3)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void SpellStoringHasAtMost5SpellLevels()
        {
            result.Type = RingConstants.SpellStoring;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2")
                .Returns("spell 3").Returns("spell 4").Returns("spell 5").Returns("spell 6");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring));
            Assert.That(ring.Contents, Contains.Item("spell 1 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 2 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 3 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 4 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 5 (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(5));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(5));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.AtLeast(5));
        }

        [Test]
        public void SpellStoringAllowsDuplicateSpells()
        {
            result.Type = RingConstants.SpellStoring;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(5));
            Assert.That(ring.Contents.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void MajorSpellStoringHasSpell()
        {
            result.Type = RingConstants.SpellStoring_Major;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(6);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 6)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Major));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 6)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void MajorSpellStoringHasAtMost10SpellLevels()
        {
            result.Type = RingConstants.SpellStoring_Major;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2")
                .Returns("spell 3").Returns("spell 4").Returns("spell 5").Returns("spell 6").Returns("spell 7").Returns("spell 8")
                .Returns("spell 9").Returns("spell 10").Returns("spell 11");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Major));
            Assert.That(ring.Contents, Contains.Item("spell 1 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 2 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 3 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 4 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 5 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 6 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 7 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 8 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 9 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 10 (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(10));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(10));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.AtLeast(10));
        }

        [Test]
        public void MajorSpellStoringAllowsDuplicateSpells()
        {
            result.Type = RingConstants.SpellStoring_Major;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Major));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(10));
            Assert.That(ring.Contents.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void CounterspellsHasSpell()
        {
            result.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(4);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 4)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Contains.Item("spell"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void CounterspellsHasAtMost1Spell()
        {
            result.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Contains.Item("spell 1"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(1));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.Exactly(1));
        }

        [Test]
        public void CounterspellsHasSpellOfAtMostSixthLevel()
        {
            result.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(6);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 6)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Contains.Item("spell"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void CounterspellsHasNoSpellHigherThanSixthLevel()
        {
            result.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(7);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 7)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Is.Empty);
        }

        [Test]
        public void GenerateCustomRing()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var ring = ringGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(ring, template);
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.Attributes, Is.EquivalentTo(attributes));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRandomCustomRing()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var ring = ringGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(ring, template);
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.Attributes, Is.EquivalentTo(attributes));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateOneTimeUseRing()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.OneTimeUse };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var ring = ringGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(ring, template);
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.Attributes, Is.EquivalentTo(attributes));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Quantity, Is.EqualTo(1));
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