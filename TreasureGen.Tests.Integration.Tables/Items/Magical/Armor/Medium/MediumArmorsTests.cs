﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Armor); }
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

        [TestCase(AttributeConstants.Shield, 1, 1, 5)]
        [TestCase(ItemTypeConstants.Armor, 1, 6, 10)]
        [TestCase(AttributeConstants.Shield, 2, 11, 20)]
        [TestCase(ItemTypeConstants.Armor, 2, 21, 30)]
        [TestCase(AttributeConstants.Shield, 3, 31, 40)]
        [TestCase(ItemTypeConstants.Armor, 3, 41, 50)]
        [TestCase(AttributeConstants.Shield, 4, 51, 55)]
        [TestCase(ItemTypeConstants.Armor, 4, 56, 57)]
        [TestCase(ItemTypeConstants.Armor, 0, 58, 60)]
        [TestCase(AttributeConstants.Shield, 0, 61, 63)]
        [TestCase("SpecialAbility", 1, 64, 100)]
        public override void TypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}