﻿using System;
using TreasureGen.Common.Items;
using TreasureGen.Tables.Interfaces;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Medium
{
    [TestFixture]
    public class MediumWeaponsTests : PercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Weapon); }
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

        [TestCase(ItemTypeConstants.Weapon, 63, 68)]
        [TestCase("SpecialAbility", 69, 100)]
        public override void Percentile(String content, Int32 lower, Int32 upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase(1, 1, 10)]
        [TestCase(2, 11, 29)]
        [TestCase(3, 30, 58)]
        [TestCase(4, 59, 62)]
        public void Percentile(Int32 bonus, Int32 lower, Int32 upper)
        {
            var content = Convert.ToString(bonus);
            Percentile(content, lower, upper);
        }
    }
}