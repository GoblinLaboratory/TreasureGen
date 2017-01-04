﻿using System;
using TreasureGen.Items;
using TreasureGen.Domain.Tables;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class MinorItemsTests : PercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Minor); }
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

        [TestCase(ItemTypeConstants.Armor, 1, 4)]
        [TestCase(ItemTypeConstants.Weapon, 5, 9)]
        [TestCase(ItemTypeConstants.Potion, 10, 44)]
        [TestCase(ItemTypeConstants.Ring, 45, 46)]
        [TestCase(ItemTypeConstants.Scroll, 47, 81)]
        [TestCase(ItemTypeConstants.Wand, 82, 91)]
        [TestCase(ItemTypeConstants.WondrousItem, 92, 100)]
        public override void Percentile(String content, Int32 lower, Int32 upper)
        {
            base.Percentile(content, lower, upper);
        }
    }
}