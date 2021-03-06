﻿using Moq;
using NUnit.Framework;
using System.Linq;
using TreasureGen.Domain.Generators.Items;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;
using TreasureGen.Items.Mundane;
using TreasureGen.Selectors.Results;

namespace TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class ItemsGeneratorTests
    {
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<IMundaneItemGeneratorRuntimeFactory> mockMundaneItemGeneratorFactory;
        private Mock<MundaneItemGenerator> mockMundaneItemGenerator;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IMagicalItemGeneratorRuntimeFactory> mockMagicalItemGeneratorFactory;
        private Mock<MagicalItemGenerator> mockMagicalItemGenerator;
        private Mock<IRangeAttributesSelector> mockRangeAttributesSelector;
        private IItemsGenerator itemsGenerator;
        private TypeAndAmountPercentileResult result;

        [SetUp]
        public void Setup()
        {
            result = new TypeAndAmountPercentileResult();
            mockMundaneItemGeneratorFactory = new Mock<IMundaneItemGeneratorRuntimeFactory>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockMagicalItemGenerator = new Mock<MagicalItemGenerator>();
            mockMagicalItemGeneratorFactory = new Mock<IMagicalItemGeneratorRuntimeFactory>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockMundaneItemGenerator = new Mock<MundaneItemGenerator>();
            mockRangeAttributesSelector = new Mock<IRangeAttributesSelector>();

            itemsGenerator = new ItemsGenerator(mockTypeAndAmountPercentileSelector.Object, mockMundaneItemGeneratorFactory.Object, mockPercentileSelector.Object, mockMagicalItemGeneratorFactory.Object, mockRangeAttributesSelector.Object);

            result.Type = "power";
            result.Amount = 42;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns(result);
            mockPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns(ItemTypeConstants.WondrousItem);

            var dummyMagicalMock = new Mock<MagicalItemGenerator>();
            dummyMagicalMock.Setup(m => m.GenerateAtPower(It.IsAny<string>())).Returns(() => new Item { Name = "magical item" });
            mockMagicalItemGeneratorFactory.Setup(f => f.CreateGeneratorOf(It.IsAny<string>())).Returns(dummyMagicalMock.Object);

            var dummyMundaneMock = new Mock<MundaneItemGenerator>();
            dummyMundaneMock.Setup(m => m.Generate()).Returns(() => new Item { Name = "mundane item" });
            mockMundaneItemGeneratorFactory.Setup(f => f.CreateGeneratorOf(It.IsAny<string>())).Returns(dummyMundaneMock.Object);

            var range = new RangeAttributesResult { Maximum = 0, Minimum = 0 };
            mockRangeAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.EpicItems, It.IsAny<string>())).Returns(range);
        }

        [Test]
        public void ItemsAreGenerated()
        {
            var items = itemsGenerator.GenerateAtLevel(1);
            Assert.That(items, Is.Not.Null);
        }

        [Test]
        public void GetItemTypeFromSelector()
        {
            itemsGenerator.GenerateAtLevel(9266);
            var expectedTableName = string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, 9266);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(expectedTableName), Times.Once);
        }

        [Test]
        public void GetAmountFromRoll()
        {
            var items = itemsGenerator.GenerateAtLevel(1);
            Assert.That(items.Count(), Is.EqualTo(42));
        }

        [Test]
        public void ReturnItems()
        {
            result.Type = PowerConstants.Mundane;
            result.Amount = 2;

            var firstItem = new Item();
            var secondItem = new Item();
            mockMundaneItemGeneratorFactory.Setup(f => f.CreateGeneratorOf(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.SetupSequence(f => f.Generate()).Returns(firstItem).Returns(secondItem);

            var items = itemsGenerator.GenerateAtLevel(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IfSelectorReturnsEmptyResult_ItemsGeneratorReturnsEmptyEnumerable()
        {
            result.Type = string.Empty;
            result.Amount = 0;

            var items = itemsGenerator.GenerateAtLevel(1);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void GetMundaneItemsFromMundaneItemGenerator()
        {
            result.Type = PowerConstants.Mundane;
            result.Amount = 1;

            var mundaneItem = new Item();
            var expectedTableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, result.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(expectedTableName)).Returns("mundane item type");
            mockMagicalItemGeneratorFactory.Setup(f => f.CreateGeneratorOf("mundane item type")).Returns(mockMagicalItemGenerator.Object);
            mockMundaneItemGeneratorFactory.Setup(f => f.CreateGeneratorOf("mundane item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.Generate()).Returns(mundaneItem);

            var items = itemsGenerator.GenerateAtLevel(1);
            Assert.That(items.Single(), Is.EqualTo(mundaneItem));
        }

        [Test]
        public void GetMagicalItemsFromMagicalItemGenerator()
        {
            result.Type = "power";
            result.Amount = 1;

            var magicalItem = new Item();
            var expectedTableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, result.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(expectedTableName)).Returns("magic item type");
            mockMagicalItemGeneratorFactory.Setup(f => f.CreateGeneratorOf("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateAtPower(result.Type)).Returns(magicalItem);

            var items = itemsGenerator.GenerateAtLevel(1);
            Assert.That(items.Single(), Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateEpicItems()
        {
            var range = new RangeAttributesResult { Maximum = 600, Minimum = 600 };
            mockRangeAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.EpicItems, "9266")).Returns(range);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Major);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("epic");

            var epicMagicalMock = new Mock<MagicalItemGenerator>();
            epicMagicalMock.Setup(m => m.GenerateAtPower(It.IsAny<string>())).Returns(() => new Item { Name = "epic item" });
            mockMagicalItemGeneratorFactory.Setup(f => f.CreateGeneratorOf("epic")).Returns(epicMagicalMock.Object);

            var items = itemsGenerator.GenerateAtLevel(9266);
            Assert.That(items.Count(), Is.EqualTo(642));
            Assert.That(items.Count(i => i.Name == "epic item"), Is.EqualTo(600));
            Assert.That(items.Count(i => i.Name == "magical item"), Is.EqualTo(42));
            Assert.That(items, Is.Unique);
        }

        [Test]
        public void GenerateOnlyEpicItems()
        {
            result.Type = string.Empty;
            result.Amount = 0;

            var range = new RangeAttributesResult { Maximum = 600, Minimum = 600 };
            mockRangeAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.EpicItems, "9266")).Returns(range);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Major);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("epic");

            var epicMagicalMock = new Mock<MagicalItemGenerator>();
            epicMagicalMock.Setup(m => m.GenerateAtPower(It.IsAny<string>())).Returns(() => new Item { Name = "epic item" });
            mockMagicalItemGeneratorFactory.Setup(f => f.CreateGeneratorOf("epic")).Returns(epicMagicalMock.Object);

            var items = itemsGenerator.GenerateAtLevel(9266);
            Assert.That(items.Count(), Is.EqualTo(600));
            Assert.That(items.Count(i => i.Name == "epic item"), Is.EqualTo(600));
            Assert.That(items, Is.Unique);
        }
    }
}