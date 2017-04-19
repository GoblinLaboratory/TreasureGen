﻿using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Mundane;
using TreasureGen.Domain.Items.Mundane;
using TreasureGen.Items;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Tests.Unit.Generators.Items.Mundane
{
    [TestFixture]
    public class MudnaneItemGeneratorSpecialMaterialDecoratorTests
    {
        private MundaneItemGenerator decorator;
        private Mock<ISpecialMaterialGenerator> mockMaterialGenerator;
        private Mock<MundaneItemGenerator> mockInnerGenerator;
        private Item item;

        [SetUp]
        public void Setup()
        {
            mockMaterialGenerator = new Mock<ISpecialMaterialGenerator>();
            mockInnerGenerator = new Mock<MundaneItemGenerator>();
            item = new Item();
            decorator = new MundaneItemGeneratorSpecialMaterialDecorator(mockInnerGenerator.Object, mockMaterialGenerator.Object);

            mockInnerGenerator.Setup(g => g.Generate()).Returns(item);
        }

        [Test]
        public void GetItemFromInnerGenerator()
        {
            var decoratedItem = decorator.Generate();
            Assert.That(decoratedItem, Is.EqualTo(item));
        }

        [Test]
        public void DoNotGetSpecialMaterial()
        {
            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.Generate();
            Assert.That(decoratedItem.Traits, Is.Not.Contains("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSpecialMaterial()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.Generate();
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetMultipleSpecialMaterials()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(true).Returns(false);
            mockMaterialGenerator.SetupSequence(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material 1").Returns("special material 2").Returns("special material 3");

            var decoratedItem = decorator.Generate();
            Assert.That(decoratedItem.Traits, Contains.Item("special material 1"));
            Assert.That(decoratedItem.Traits, Contains.Item("special material 2"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(2));
        }

        [Test]
        public void DragonhideIsNotMetal()
        {
            item.Attributes = new[] { AttributeConstants.Metal };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.Generate();
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
            Assert.That(decoratedItem.Attributes, Is.Not.Contains(AttributeConstants.Metal));
        }

        [Test]
        public void DragonhideIsNotWood()
        {
            item.Attributes = new[] { AttributeConstants.Wood };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.Generate();
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
            Assert.That(decoratedItem.Attributes, Is.Not.Contains(AttributeConstants.Wood));
        }

        [Test]
        public void DecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.Generate(template, true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void DoNotDecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, false)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.Generate(template);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Is.Empty);
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
            Assert.That(() => decorator.GenerateFromSubset(Enumerable.Empty<string>()), Throws.ArgumentException.With.Message.EqualTo("Cannot generate from an empty collection subset"));
        }
    }
}