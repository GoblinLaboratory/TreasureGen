﻿using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class WondrousItemGeneratorTests : StressTests
    {
        [Inject, Named(ItemTypeConstants.WondrousItem)]
        public IMagicalItemGenerator WondrousItemGenerator { get; set; }

        protected override void StressGenerator()
        {
            var power = GetNewPower(false);
            var item = WondrousItemGenerator.GenerateAtPower(power);

            Assert.That(item.Name, Is.Not.Empty);
            Assert.That(item.Traits, Is.Not.Null);
            Assert.That(item.Attributes, Is.Not.Null);
            Assert.That(item.Quantity, Is.EqualTo(1));
            Assert.That(item.Magic, Is.Not.Empty);
        }
    }
}