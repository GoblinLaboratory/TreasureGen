﻿using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using EquipmentGen.Generators.RuntimeFactories.Interfaces;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class WondrousItemGeneratorTests : StressTests
    {
        [Inject]
        public IMagicalItemGeneratorFactory MagicalItemGeneratorFactory { get; set; }

        private IMagicalItemGenerator wondrousItemGenerator;

        [SetUp]
        public void Setup()
        {
            wondrousItemGenerator = MagicalItemGeneratorFactory.CreateWith(ItemTypeConstants.WondrousItem);

            StartTest();
        }

        [TearDown]
        public void TearDown()
        {
            StopTest();
        }

        [Test]
        public void StressedWondrousItemGenerator()
        {
            while (TestShouldKeepRunning())
            {
                var power = GetNewPower(false);
                var item = wondrousItemGenerator.GenerateAtPower(power);

                Assert.That(item.Name, Is.Not.Empty);
                Assert.That(item.Traits, Is.Not.Null);
                Assert.That(item.Attributes, Is.Not.Null);
                Assert.That(item.Quantity, Is.EqualTo(1));
                Assert.That(item.Magic, Is.Not.Empty);
            }

            AssertIterations();
        }
    }
}