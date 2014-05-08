﻿using System;
using EquipmentGen.Common.Items;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Common.Items
{
    [TestFixture]
    public class ItemTypeConstantsTests
    {
        [TestCase(ItemTypeConstants.AlchemicalItem, "AlchemicalItem")]
        [TestCase(ItemTypeConstants.Armor, "Armor")]
        [TestCase(ItemTypeConstants.Weapon, "Weapon")]
        [TestCase(ItemTypeConstants.Tool, "Tool")]
        [TestCase(ItemTypeConstants.Potion, "Potion")]
        [TestCase(ItemTypeConstants.Ring, "Ring")]
        [TestCase(ItemTypeConstants.Rod, "Rod")]
        [TestCase(ItemTypeConstants.Scroll, "Scroll")]
        [TestCase(ItemTypeConstants.Staff, "Staff")]
        [TestCase(ItemTypeConstants.Wand, "Wand")]
        [TestCase(ItemTypeConstants.WondrousItem, "WondrousItem")]
        [TestCase(ItemTypeConstants.SpecificCursedItem, "SpecificCursedItem")]
        public void Constant(String constant, String value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}