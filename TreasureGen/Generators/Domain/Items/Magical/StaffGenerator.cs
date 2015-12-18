﻿using System;
using System.Linq;
using TreasureGen.Common.Items;
using TreasureGen.Generators.Items.Magical;
using TreasureGen.Selectors;
using TreasureGen.Tables;

namespace TreasureGen.Generators.Domain.Items.Magical
{
    public class StaffGenerator : MagicalItemGenerator
    {
        private IPercentileSelector percentileSelector;
        private IChargesGenerator chargesGenerator;
        private IAttributesSelector attributesSelector;

        public StaffGenerator(IPercentileSelector percentileSelector, IChargesGenerator chargesGenerator, IAttributesSelector attributesSelector)
        {
            this.percentileSelector = percentileSelector;
            this.chargesGenerator = chargesGenerator;
            this.attributesSelector = attributesSelector;
        }

        public Item GenerateAtPower(String power)
        {
            if (power == PowerConstants.Minor)
                throw new ArgumentException("Cannot generate minor staves");

            var tablename = String.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            var staffName = percentileSelector.SelectFrom(tablename);

            var staff = new Item();
            staff.Name = staffName;
            staff.ItemType = ItemTypeConstants.Staff;
            staff.Magic.Charges = chargesGenerator.GenerateFor(staff.ItemType, staffName);
            staff.Attributes = new[] { AttributeConstants.OneTimeUse, AttributeConstants.Charged };

            if (staffName != StaffConstants.Power)
                return staff;

            staff.Magic.Bonus = 2;
            tablename = String.Format(TableNameConstants.Attributes.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Weapon);
            var quarterstaffAttributes = attributesSelector.SelectFrom(tablename, WeaponConstants.Quarterstaff);
            staff.Attributes = staff.Attributes.Union(quarterstaffAttributes);

            return staff;
        }
    }
}