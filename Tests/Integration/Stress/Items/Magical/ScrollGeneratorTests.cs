﻿using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class ScrollGeneratorTests : StressTests
    {
        [Inject, Named(ItemTypeConstants.Scroll)]
        public IMagicalItemGenerator ScrollGenerator { get; set; }

        [Test]
        public void StressedScrollGenerator()
        {
            StressGenerator();
        }

        protected override void MakeAssertions()
        {
            var power = GetNewPower(false);
            var scroll = ScrollGenerator.GenerateAtPower(power);

            Assert.That(scroll.Name, Contains.Item(" scroll of "));
            Assert.That(scroll.Traits, Is.Not.Null);
            Assert.That(scroll.Attributes, Is.Not.Null);
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.Magic[Magic.IsMagical], Is.True);
        }
    }
}