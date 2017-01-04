﻿using System;
using TreasureGen.Items;
using TreasureGen.Domain.Tables;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class MediumItemsTests : PercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Medium); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase(ItemTypeConstants.Armor, 1, 10)]
        [TestCase(ItemTypeConstants.Weapon, 11, 20)]
        [TestCase(ItemTypeConstants.Potion, 21, 30)]
        [TestCase(ItemTypeConstants.Ring, 31, 40)]
        [TestCase(ItemTypeConstants.Rod, 41, 50)]
        [TestCase(ItemTypeConstants.Scroll, 51, 65)]
        [TestCase(ItemTypeConstants.Staff, 66, 68)]
        [TestCase(ItemTypeConstants.Wand, 69, 83)]
        [TestCase(ItemTypeConstants.WondrousItem, 84, 100)]
        public override void Percentile(String content, Int32 lower, Int32 upper)
        {
            base.Percentile(content, lower, upper);
        }
    }
}