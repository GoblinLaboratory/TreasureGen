﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Staves
{
    [TestFixture]
    public class MediumStaffsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Staff); }
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

        [TestCase(StaffConstants.Charming, 1, 15)]
        [TestCase(StaffConstants.Fire, 16, 30)]
        [TestCase(StaffConstants.SwarmingInsects, 31, 40)]
        [TestCase(StaffConstants.Healing, 41, 60)]
        [TestCase(StaffConstants.SizeAlteration, 61, 75)]
        [TestCase(StaffConstants.Illumination, 76, 90)]
        [TestCase(StaffConstants.Frost, 91, 95)]
        [TestCase(StaffConstants.Defense, 96, 100)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }
    }
}