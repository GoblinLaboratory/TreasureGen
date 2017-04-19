﻿using Moq;
using NUnit.Framework;
using RollGen;
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
    public class WondrousItemGeneratorTests
    {
        private MagicalItemGenerator wondrousItemGenerator;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IMagicalItemTraitsGenerator> mockTraitsGenerator;
        private Mock<ICollectionsSelector> mockAttributesSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<Dice> mockDice;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private TypeAndAmountPercentileResult result;
        private string power;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockTraitsGenerator = new Mock<IMagicalItemTraitsGenerator>();
            mockAttributesSelector = new Mock<ICollectionsSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockDice = new Mock<Dice>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            result = new TypeAndAmountPercentileResult();
            wondrousItemGenerator = new WondrousItemGenerator(mockPercentileSelector.Object, mockAttributesSelector.Object, mockChargesGenerator.Object, mockDice.Object, mockSpellGenerator.Object, mockTypeAndAmountPercentileSelector.Object);
            itemVerifier = new ItemVerifier();

            power = "power";
            result.Type = "wondrous item";
            result.Amount = 0;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.WondrousItem);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(result);
        }

        [Test]
        public void GenerateWondrousItem()
        {
            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(result.Type));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "type 1", "type 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.WondrousItem);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, result.Type)).Returns(attributes);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void DoNotGetChargesIfNotCharged()
        {
            var attributes = new[] { "type 1", "type 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.WondrousItem);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, result.Type)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, "wondrous item")).Returns(9266);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void GetChargesIfCharged()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.WondrousItem);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, result.Type)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, result.Type)).Returns(9266);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GetBonus()
        {
            result.Amount = 90210;
            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Magic.Bonus, Is.EqualTo(90210));
        }

        [Test]
        public void HornOfValhallaGetsType()
        {
            result.Type = WondrousItemConstants.HornOfValhalla;
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.HornOfValhallaTypes)).Returns("metallic");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.HornOfValhalla));
            Assert.That(item.Traits, Contains.Item("metallic"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void IronFlaskContentsGenerated()
        {
            result.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IronFlaskContents)).Returns("contents");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Contains.Item("contents"));
        }

        [Test]
        public void IronFlaskContentsDoNotContainEmptyString()
        {
            result.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IronFlaskContents)).Returns(String.Empty);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Is.Empty);
        }

        [Test]
        public void IronFlaskOnlyContainsOneThing()
        {
            result.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IronFlaskContents)).Returns("contents").Returns("more contents");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Contains.Item("contents"));
            Assert.That(item.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfBalorOrPitFiend_GetFromSelector()
        {
            result.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.IronFlaskContents)).Returns(TableNameConstants.Percentiles.Set.BalorOrPitFiend);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.BalorOrPitFiend)).Returns("balor or pit fiend");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Contains.Item("balor or pit fiend"));
        }

        [Test]
        public void RobeOfUsefulItemsBaseItemsAdded()
        {
            SetUpRoll(4, 4, 0);
            result.Type = WondrousItemConstants.RobeOfUsefulItems;
            var items = new[] { "item 1", "item 1", "item 2", "item 2", "item 3", "item 3" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, result.Type)).Returns(items);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            foreach (var baseItem in items)
                Assert.That(item.Contents, Contains.Item(baseItem));
        }

        [Test]
        public void RobeOfUsefulItemsExtraItemsDetermined()
        {
            result.Type = WondrousItemConstants.RobeOfUsefulItems;
            SetUpRoll(4, 4, 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(TableNameConstants.Percentiles.Set.RobeOfUsefulItemsExtraItems)).Returns("item 1").Returns("item 2");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("item 1"));
            Assert.That(item.Contents, Contains.Item("item 2"));
            Assert.That(item.Contents.Count, Is.EqualTo(2));
        }

        [Test]
        public void RobeOfUsefulItemsCanHaveDuplicateExtraItems()
        {
            result.Type = WondrousItemConstants.RobeOfUsefulItems;
            SetUpRoll(4, 4, 2);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.RobeOfUsefulItemsExtraItems)).Returns("item 1");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("item 1"));
            Assert.That(item.Contents.Count(i => i == "item 1"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(2));
        }

        [Test]
        public void RobeOfUsefulItemsExtraItemsScrollDetermined()
        {
            result.Type = WondrousItemConstants.RobeOfUsefulItems;
            SetUpRoll(4, 4, 1);
            mockPercentileSelector.Setup(p => p.SelectFrom(TableNameConstants.Percentiles.Set.RobeOfUsefulItemsExtraItems)).Returns("Scroll");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(9266);
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.Generate("spell type", 9266)).Returns("spell");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("spell type scroll of spell (9266)"));
        }

        [Test]
        public void CubicGateGetsPlanes()
        {
            result.Type = WondrousItemConstants.CubicGate;
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(TableNameConstants.Percentiles.Set.Planes)).Returns("plane 1").Returns("plane 2").Returns("plane 3")
                .Returns("plane 4").Returns("plane 5");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("Material Plane"));
            Assert.That(item.Contents, Contains.Item("plane 1"));
            Assert.That(item.Contents, Contains.Item("plane 2"));
            Assert.That(item.Contents, Contains.Item("plane 3"));
            Assert.That(item.Contents, Contains.Item("plane 4"));
            Assert.That(item.Contents, Contains.Item("plane 5"));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
        }

        [Test]
        public void CubicGateGetsDistinctPlanes()
        {
            result.Type = WondrousItemConstants.CubicGate;
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(TableNameConstants.Percentiles.Set.Planes)).Returns("plane 1").Returns("plane 1").Returns("plane 2")
                .Returns("plane 3").Returns("plane 4").Returns("plane 5");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("Material Plane"));
            Assert.That(item.Contents, Contains.Item("plane 1"));
            Assert.That(item.Contents, Contains.Item("plane 2"));
            Assert.That(item.Contents, Contains.Item("plane 3"));
            Assert.That(item.Contents, Contains.Item("plane 4"));
            Assert.That(item.Contents, Contains.Item("plane 5"));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
        }

        [Test]
        public void GetCardsForDeckOfIllusions()
        {
            result.Type = WondrousItemConstants.DeckOfIllusions;
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, result.Type)).Returns(3);
            var cards = new[] { "card 1", "card 2", "card 3", "card 4" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, result.Type)).Returns(cards);
            SetUpRoll(1, 4, 1);
            SetUpRoll(1, 3, 1);
            SetUpRoll(1, 2, 2);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("card 1"));
            Assert.That(item.Contents, Contains.Item("card 2"));
            Assert.That(item.Contents, Contains.Item("card 4"));
            Assert.That(item.Contents.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSpheresForNecklaceOfFireballs()
        {
            result.Type = WondrousItemConstants.NecklaceOfFireballs + " type whatever";
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, result.Type)).Returns(4);
            var spheres = new[] { "small sphere", "big sphere", "normal sphere", "normal sphere", "big sphere" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, result.Type)).Returns(spheres);
            SetUpRoll(1, 5, 1);
            SetUpRoll(1, 4, 1);
            SetUpRoll(1, 3, 1);
            SetUpRoll(1, 2, 2);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("small sphere"));
            Assert.That(item.Contents, Contains.Item("normal sphere"));
            Assert.That(item.Contents, Contains.Item("big sphere"));
            Assert.That(item.Contents.Count(c => c == "big sphere"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(4));
        }

        private void SetUpRoll(int quantity, int die, int result)
        {
            var mockPartial = new Mock<PartialRoll>();
            mockPartial.Setup(p => p.AsSum()).Returns(result);
            mockDice.Setup(d => d.Roll(quantity).d(die)).Returns(mockPartial.Object);
        }

        [Test]
        public void RobeOfBonesItemsAdded()
        {
            result.Type = WondrousItemConstants.RobeOfBones;
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, result.Type)).Returns(4);
            var items = new[] { "undead 1", "undead 1", "undead 2", "undead 2", "undead 3", "undead 3" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, result.Type)).Returns(items);
            SetUpRoll(1, 6, 1);
            SetUpRoll(1, 5, 1);
            SetUpRoll(1, 4, 1);
            SetUpRoll(1, 3, 2);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("undead 1"));
            Assert.That(item.Contents.Count(c => c == "undead 1"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 2"));
            Assert.That(item.Contents, Contains.Item("undead 3"));
            Assert.That(item.Contents.Count, Is.EqualTo(4));
        }

        [Test]
        public void DoNotRollIfFullContents()
        {
            result.Type = WondrousItemConstants.RobeOfBones;
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, result.Type)).Returns(6);
            var items = new[] { "undead 1", "undead 1", "undead 2", "undead 2", "undead 3", "undead 3" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, result.Type)).Returns(items);

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Contents, Contains.Item("undead 1"));
            Assert.That(item.Contents.Count(c => c == "undead 1"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 2"));
            Assert.That(item.Contents.Count(c => c == "undead 2"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 3"));
            Assert.That(item.Contents.Count(c => c == "undead 3"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void CandleOfInvocationGetsAlignment()
        {
            result.Type = WondrousItemConstants.CandleOfInvocation;
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments)).Returns("alignment");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Traits, Contains.Item("alignment"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void RobeOfTheArchmagiGetsAlignment()
        {
            result.Type = WondrousItemConstants.RobeOfTheArchmagi;
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.RobeOfTheArchmagiColors)).Returns("color (alignment)");

            var item = wondrousItemGenerator.GenerateAtPower(power);
            Assert.That(item.Name, Is.EqualTo(result.Type));
            Assert.That(item.Traits, Contains.Item("color (alignment)"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateCustomWondrousItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "type 1", "type 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.WondrousItem);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var wondrousItem = wondrousItemGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(wondrousItem, template);
            Assert.That(wondrousItem.Name, Is.EqualTo(name));
            Assert.That(wondrousItem.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(wondrousItem.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(wondrousItem.IsMagical, Is.True);
            Assert.That(wondrousItem.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateRandomCustomWondrousItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "type 1", "type 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.WondrousItem);
            mockAttributesSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var wondrousItem = wondrousItemGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(wondrousItem, template);
            Assert.That(wondrousItem.Name, Is.EqualTo(name));
            Assert.That(wondrousItem.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(wondrousItem.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(wondrousItem.IsMagical, Is.True);
            Assert.That(wondrousItem.Attributes, Is.EqualTo(attributes));
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