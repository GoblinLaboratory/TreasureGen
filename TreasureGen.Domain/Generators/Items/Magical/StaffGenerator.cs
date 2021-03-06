﻿using System;
using System.Linq;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class StaffGenerator : MagicalItemGenerator
    {
        private IPercentileSelector percentileSelector;
        private IChargesGenerator chargesGenerator;
        private ICollectionsSelector attributesSelector;
        private ISpecialAbilitiesGenerator specialAbilitiesGenerator;

        public StaffGenerator(IPercentileSelector percentileSelector, IChargesGenerator chargesGenerator, ICollectionsSelector attributesSelector, ISpecialAbilitiesGenerator specialAbilitiesGenerator)
        {
            this.percentileSelector = percentileSelector;
            this.chargesGenerator = chargesGenerator;
            this.attributesSelector = attributesSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
        }

        public Item GenerateAtPower(string power)
        {
            if (power == PowerConstants.Minor)
                throw new ArgumentException("Cannot generate minor staves");

            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            var staffName = percentileSelector.SelectFrom(tablename);

            var staff = new Item();
            staff.Name = staffName;
            staff.ItemType = ItemTypeConstants.Staff;
            staff.Attributes = new[] { AttributeConstants.OneTimeUse, AttributeConstants.Charged };
            staff = BuildStaff(staff);
            staff.Magic.Charges = chargesGenerator.GenerateFor(staff.ItemType, staffName);

            if (staff.Name != StaffConstants.Power)
                return staff;

            staff.Magic.Bonus = 2;

            return staff;
        }

        private Item BuildStaff(Item staff)
        {
            staff.Quantity = 1;
            staff.IsMagical = true;

            if (staff.Name != StaffConstants.Power)
                return staff;

            var tablename = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Weapon);
            var quarterstaffAttributes = attributesSelector.SelectFrom(tablename, WeaponConstants.Quarterstaff);
            staff.Attributes = staff.Attributes.Union(quarterstaffAttributes).Except(new[] { AttributeConstants.OneTimeUse });

            return staff;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            template.ItemType = ItemTypeConstants.Staff;
            template.Attributes = new[] { AttributeConstants.Charged, AttributeConstants.OneTimeUse };

            var staff = template.Copy();
            staff = BuildStaff(staff);

            if (staff.CanBeUsedAsWeaponOrArmor)
            {
                staff.Magic.Intelligence = template.Magic.Intelligence.Copy();

                var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
                staff.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(specialAbilityNames);
            }

            return staff;
        }
    }
}