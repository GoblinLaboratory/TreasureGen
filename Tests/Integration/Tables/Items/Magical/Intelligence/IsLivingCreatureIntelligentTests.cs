﻿using System;
using TreasureGen.Common.Items;
using TreasureGen.Tables.Interfaces;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IsLivingCreatureIntelligentTests : BooleanPercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, ItemTypeConstants.LivingCreature); }
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

        [TestCase(false, 1, 100)]
        public override void BooleanPercentile(Boolean isTrue, Int32 lower, Int32 upper)
        {
            base.BooleanPercentile(isTrue, lower, upper);
        }
    }
}