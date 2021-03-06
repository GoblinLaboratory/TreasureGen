﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class SpecificCursedItemsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.SpecificCursedItems; }
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

        [TestCase(WondrousItemConstants.IncenseOfObsession, 1, 5)]
        [TestCase(RingConstants.Clumsiness, 6, 15)]
        [TestCase(WondrousItemConstants.AmuletOfInescapableLocation, 16, 20)]
        [TestCase(WondrousItemConstants.StoneOfWeight_Loadstone, 21, 25)]
        [TestCase(WondrousItemConstants.BracersOfDefenselessness, 26, 30)]
        [TestCase(WondrousItemConstants.GauntletsOfFumbling, 31, 35)]
        [TestCase(WeaponConstants.CursedMinus2Sword, 36, 40)]
        [TestCase(ArmorConstants.ArmorOfRage, 41, 43)]
        [TestCase(WondrousItemConstants.MedallionOfThoughtProjection, 44, 46)]
        [TestCase(WondrousItemConstants.FlaskOfCurses, 47, 52)]
        [TestCase(WondrousItemConstants.DustOfSneezingAndChoking, 53, 54)]
        [TestCase(PotionConstants.Poison, 56, 60)]
        [TestCase(WondrousItemConstants.RobeOfPowerlessness, 62, 63)]
        [TestCase(WeaponConstants.CursedBackbiterSpear, 65, 68)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction, 69, 70)]
        [TestCase(WeaponConstants.NetOfSnaring, 71, 72)]
        [TestCase(WondrousItemConstants.BagOfDevouring, 73, 75)]
        [TestCase(WeaponConstants.MaceOfBlood, 76, 80)]
        [TestCase(WondrousItemConstants.RobeOfVermin, 81, 85)]
        [TestCase(WondrousItemConstants.PeriaptOfFoulRotting, 86, 88)]
        [TestCase(WeaponConstants.BerserkingSword, 89, 92)]
        [TestCase(WondrousItemConstants.BootsOfDancing, 93, 96)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase(WondrousItemConstants.HelmOfOppositeAlignment, 55)]
        [TestCase(WondrousItemConstants.BroomOfAnimatedAttack, 61)]
        [TestCase(WondrousItemConstants.VacousGrimoire, 64)]
        [TestCase(WondrousItemConstants.CrystalBall_Hypnosis, 97)]
        [TestCase(WondrousItemConstants.NecklaceOfStrangulation, 98)]
        [TestCase(WondrousItemConstants.CloakOfPoisonousness, 99)]
        [TestCase(WondrousItemConstants.ScarabOfDeath, 100)]
        public override void Percentile(string content, int roll)
        {
            base.Percentile(content, roll);
        }
    }
}