﻿using System;
using EquipmentGen.Common.Coins;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level20CoinsTests : TypeAndAmountPercentileTests
    {
        protected override String tableName
        {
            get { return "Level20Coins"; }
        }

        [TestCase(EmptyContent, 1, 2)]
        public override void Percentile(String content, Int32 lower, Int32 upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase(CoinConstants.Gold, "4d8*1000", 3, 65)]
        [TestCase(CoinConstants.Platinum, "4d10*100", 66, 100)]
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