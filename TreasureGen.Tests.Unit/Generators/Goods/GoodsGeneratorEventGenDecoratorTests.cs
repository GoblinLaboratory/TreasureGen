﻿using EventGen;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TreasureGen.Domain.Generators.Goods;
using TreasureGen.Goods;

namespace TreasureGen.Tests.Unit.Generators.Goods
{
    [TestFixture]
    public class GoodsGeneratorEventGenDecoratorTests
    {
        private IGoodsGenerator decorator;
        private Mock<IGoodsGenerator> mockInnerGenerator;
        private Mock<GenEventQueue> mockEventQueue;

        [SetUp]
        public void Setup()
        {
            mockInnerGenerator = new Mock<IGoodsGenerator>();
            mockEventQueue = new Mock<GenEventQueue>();
            decorator = new GoodsGeneratorEventGenDecorator(mockInnerGenerator.Object, mockEventQueue.Object);
        }

        [Test]
        public void LogGenerationEvents()
        {
            var innerGoods = new List<Good>();
            innerGoods.Add(new Good());
            innerGoods.Add(new Good());

            mockInnerGenerator.Setup(g => g.GenerateAtLevel(9266)).Returns(innerGoods);

            var Goods = decorator.GenerateAtLevel(9266);
            Assert.That(Goods, Is.EqualTo(innerGoods));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("TreasureGen", "Beginning level 9266 goods generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("TreasureGen", "Completed generation of 2 level 9266 goods"), Times.Once);
        }
    }
}
