﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;

namespace TreasureGen.Tests.Integration.Tables.Items.Mundane
{
    [TestFixture]
    public class MundaneGearSizesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.MundaneGearSizes; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(TraitConstants.Sizes.Tiny, 1, 1)]
        [TestCase(TraitConstants.Sizes.Small, 2, 11)]
        [TestCase(TraitConstants.Sizes.Medium, 12, 87)]
        [TestCase(TraitConstants.Sizes.Large, 88, 97)]
        [TestCase(TraitConstants.Sizes.Huge, 98, 98)]
        [TestCase(TraitConstants.Sizes.Gargantuan, 99, 99)]
        [TestCase(TraitConstants.Sizes.Colossal, 100, 100)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}