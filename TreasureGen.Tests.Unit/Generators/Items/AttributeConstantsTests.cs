﻿using NUnit.Framework;
using TreasureGen.Items;

namespace TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class AttributeConstantsTests
    {
        [TestCase(AttributeConstants.Ammunition, "Ammunition")]
        [TestCase(AttributeConstants.Bludgeoning, "Bludgeoning")]
        [TestCase(AttributeConstants.Charged, "Charged")]
        [TestCase(AttributeConstants.Common, "Common")]
        [TestCase(AttributeConstants.DoubleWeapon, "Double weapon")]
        [TestCase(AttributeConstants.Melee, "Melee")]
        [TestCase(AttributeConstants.Metal, "Metal")]
        [TestCase(AttributeConstants.Piercing, "Piercing")]
        [TestCase(AttributeConstants.NotTower, "Not tower")]
        [TestCase(AttributeConstants.OneTimeUse, "One-time use")]
        [TestCase(AttributeConstants.Ranged, "Ranged")]
        [TestCase(AttributeConstants.Shield, "Shield")]
        [TestCase(AttributeConstants.Slashing, "Slashing")]
        [TestCase(AttributeConstants.Thrown, "Thrown")]
        [TestCase(AttributeConstants.TwoHanded, "Two-Handed")]
        [TestCase(AttributeConstants.Uncommon, "Uncommon")]
        [TestCase(AttributeConstants.Wood, "Wood")]
        public void Constant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}