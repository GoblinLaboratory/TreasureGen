﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level7DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXSPELLTYPESpells, 7, "Divine"); }
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

        [TestCase("Animate plants", 1, 5)]
        [TestCase("Blasphemy", 6, 9)]
        [TestCase("Changestaff", 10, 14)]
        [TestCase("Control weather", 15, 16)]
        [TestCase("Creeping doom", 17, 21)]
        [TestCase("Mass cure serious wounds", 22, 27)]
        [TestCase("Destruction", 28, 32)]
        [TestCase("Dictum", 33, 36)]
        [TestCase("Ethereal jaunt", 37, 41)]
        [TestCase("Holy word", 42, 45)]
        [TestCase("Mass inflict serious wounds", 46, 50)]
        [TestCase("Refuge", 51, 55)]
        [TestCase("Regenerate", 56, 60)]
        [TestCase("Repulsion", 61, 65)]
        [TestCase("Greater restoration", 66, 69)]
        [TestCase("Resurrection", 70, 71)]
        [TestCase("Greater scrying", 72, 76)]
        [TestCase("Summon monster VII", 77, 81)]
        [TestCase("Summon nature's ally VII", 82, 85)]
        [TestCase("Sunbeam", 86, 90)]
        [TestCase("Transmute metal to wood", 93, 97)]
        [TestCase("Word of chaos", 98, 100)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase("Symbol of stunning", 91)]
        [TestCase("Symbol of weakness", 92)]
        public override void Percentile(string content, int roll)
        {
            base.Percentile(content, roll);
        }
    }
}