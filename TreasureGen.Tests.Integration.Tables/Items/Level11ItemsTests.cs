﻿using System;
using TreasureGen.Items;
using TreasureGen.Domain.Tables;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items
{
    [TestFixture]
    public class Level11ItemsTests : TypeAndAmountPercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, 11); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(EmptyContent, 1, 31)]
        public override void Percentile(String content, Int32 lower, Int32 upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase(PowerConstants.Minor, "1d4", 32, 84)]
        [TestCase(PowerConstants.Medium, "1", 85, 98)]
        [TestCase(PowerConstants.Major, "1", 99, 100)]
        public override void TypeAndAmountPercentile(String type, String amount, Int32 lower, Int32 upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}