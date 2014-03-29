﻿using System;
using EquipmentGen.Common.Items;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Items.MagicalItems.Weapons.Medium
{
    [TestFixture]
    public class MediumMeleeWeaponSpecialAbilitiesTests : PercentileTests
    {
        protected override String GetTableName()
        {
            return "MediumMeleeWeaponSpecialAbilities";
        }

        [TestCase(SpecialAbilityConstants.Bane, 1, 6)]
        [TestCase(SpecialAbilityConstants.Defending, 7, 12)]
        [TestCase(SpecialAbilityConstants.Flaming, 13, 19)]
        [TestCase(SpecialAbilityConstants.Frost, 20, 26)]
        [TestCase(SpecialAbilityConstants.Shock, 27, 33)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon, 34, 38)]
        [TestCase(SpecialAbilityConstants.Keen, 39, 44)]
        [TestCase(SpecialAbilityConstants.KiFocus, 45, 48)]
        [TestCase(SpecialAbilityConstants.Merciful, 49, 50)]
        [TestCase(SpecialAbilityConstants.MightyCleaving, 51, 54)]
        [TestCase(SpecialAbilityConstants.SpellStoring, 55, 59)]
        [TestCase(SpecialAbilityConstants.Throwing, 60, 63)]
        [TestCase(SpecialAbilityConstants.Thundering, 64, 65)]
        [TestCase(SpecialAbilityConstants.Vicious, 66, 69)]
        [TestCase(SpecialAbilityConstants.Anarchic, 70, 72)]
        [TestCase(SpecialAbilityConstants.Axiomatic, 73, 75)]
        [TestCase(SpecialAbilityConstants.Disruption, 76, 78)]
        [TestCase(SpecialAbilityConstants.FlamingBurst, 79, 81)]
        [TestCase(SpecialAbilityConstants.IcyBurst, 82, 84)]
        [TestCase(SpecialAbilityConstants.Holy, 85, 87)]
        [TestCase(SpecialAbilityConstants.ShockingBurst, 88, 90)]
        [TestCase(SpecialAbilityConstants.Unholy, 91, 93)]
        [TestCase(SpecialAbilityConstants.Wounding, 94, 95)]
        [TestCase("BonusSpecialAbility", 96, 100)]
        public void Percentile(String content, Int32 lower, Int32 upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}