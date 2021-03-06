﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;

namespace TreasureGen.Tests.Integration.Tables.Items.Mundane.Armors
{
    [TestFixture]
    public class MundaneShieldsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.MundaneShields; }
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

        [TestCase(ArmorConstants.Buckler, 1, 17)]
        [TestCase(ArmorConstants.LightWoodenShield, 18, 40)]
        [TestCase(ArmorConstants.LightSteelShield, 41, 60)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 61, 83)]
        [TestCase(ArmorConstants.HeavySteelShield, 84, 100)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }
    }
}