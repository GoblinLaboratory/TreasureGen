﻿using System;
using EquipmentGen.Generators.Interfaces.Items;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Items
{
    [TestFixture]
    public class ItemsGeneratorTests : StressTests
    {
        [Inject]
        public IItemsGenerator ItemsGenerator { get; set; }

        [SetUp]
        public void Setup()
        {
            StartTest();
        }

        [TearDown]
        public void TearDown()
        {
            StopTest();
        }

        [Test]
        public void StressedItemsGeneratorAtLevel()
        {
            while (TestShouldKeepRunning())
            {
                var level = GetNewLevel();
                Console.WriteLine("Level: {0}", level);
                var items = ItemsGenerator.GenerateAtLevel(level);

                Assert.That(items, Is.Not.Null);
                foreach (var item in items)
                {
                    Assert.That(item.Name, Is.Not.Empty);
                    Assert.That(item.Attributes, Is.Not.Null);
                    Assert.That(item.Magic, Is.Not.Null);
                    Assert.That(item.Quantity, Is.GreaterThan(0));
                    Assert.That(item.Traits, Is.Not.Null);
                }
            }

            AssertIterations();
        }

        [Test]
        public void StressedItemsGeneratorAtPower()
        {
            while (TestShouldKeepRunning())
            {
                var power = GetNewPower(true);
                var item = ItemsGenerator.GenerateAtPower(power);

                Assert.That(item.Name, Is.Not.Empty);
                Assert.That(item.Attributes, Is.Not.Null);
                Assert.That(item.Magic, Is.Not.Null);
                Assert.That(item.Quantity, Is.GreaterThan(0));
                Assert.That(item.Traits, Is.Not.Null);
            }

            AssertIterations();
        }
    }
}