﻿using Moq;
using NUnit.Framework;
using System;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalItemGeneratorCurseDecoratorTests
    {
        private MagicalItemGenerator decorator;
        private Mock<ICurseGenerator> mockCurseGenerator;
        private Mock<MagicalItemGenerator> mockInnerGenerator;
        private Item innerItem;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockInnerGenerator = new Mock<MagicalItemGenerator>();
            mockCurseGenerator = new Mock<ICurseGenerator>();
            decorator = new MagicalItemGeneratorCurseDecorator(mockInnerGenerator.Object, mockCurseGenerator.Object);
            innerItem = new Item();
            itemVerifier = new ItemVerifier();

            mockInnerGenerator.Setup(g => g.GenerateAtPower("power")).Returns(innerItem);
        }

        [Test]
        public void GetItemFromInnerGenerator()
        {
            var item = decorator.GenerateAtPower("power");
            Assert.That(item, Is.EqualTo(innerItem));
        }

        [Test]
        public void DoNotGetCurseIfNotCursed()
        {
            mockCurseGenerator.Setup(g => g.HasCurse(It.IsAny<bool>())).Returns(false);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var item = decorator.GenerateAtPower("power");
            Assert.That(item.Magic.Curse, Is.Empty);
        }

        [Test]
        public void GetCurseIfCursed()
        {
            mockCurseGenerator.Setup(g => g.HasCurse(It.IsAny<bool>())).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var item = decorator.GenerateAtPower("power");
            Assert.That(item.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void GetSpecificCursedItems()
        {
            var specificCursedItem = new Item();
            mockCurseGenerator.Setup(g => g.HasCurse(It.IsAny<bool>())).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns(TableNameConstants.Percentiles.Set.SpecificCursedItems);
            mockCurseGenerator.Setup(g => g.GenerateSpecificCursedItem()).Returns(specificCursedItem);

            var item = decorator.GenerateAtPower("power");
            Assert.That(item, Is.EqualTo(specificCursedItem));
        }

        [Test]
        public void DecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(innerItem);
            mockCurseGenerator.Setup(g => g.HasCurse(It.IsAny<bool>())).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(innerItem));
            Assert.That(decoratedItem.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void DoNotDecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, false)).Returns(innerItem);
            mockCurseGenerator.Setup(g => g.HasCurse(It.IsAny<bool>())).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var decoratedItem = decorator.Generate(template);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(innerItem));
            Assert.That(decoratedItem.Magic.Curse, Is.Empty);
        }

        [Test]
        public void DoNotGetSpecificCursedItemsForCustomItem()
        {
            var template = new Item();
            var specificCursedItem = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(innerItem);
            mockCurseGenerator.Setup(g => g.HasCurse(It.IsAny<bool>())).Returns(true);
            mockCurseGenerator.SetupSequence(g => g.GenerateCurse())
                .Returns(TableNameConstants.Percentiles.Set.SpecificCursedItems)
                .Returns("cursed");
            mockCurseGenerator.Setup(g => g.GenerateSpecificCursedItem()).Returns(specificCursedItem);

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.Not.EqualTo(specificCursedItem));
            Assert.That(decoratedItem, Is.EqualTo(innerItem));
            Assert.That(decoratedItem.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void GenerateSpecificCursedCustomItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var cursedItem = itemVerifier.CreateRandomTemplate(name);
            mockCurseGenerator.Setup(g => g.IsSpecificCursedItem(template)).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateSpecificCursedItem(template)).Returns(cursedItem);

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.EqualTo(cursedItem));
            mockInnerGenerator.Verify(g => g.Generate(It.IsAny<Item>(), It.IsAny<bool>()), Times.Never);
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