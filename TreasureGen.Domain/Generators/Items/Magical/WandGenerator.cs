﻿using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class WandGenerator : MagicalItemGenerator
    {
        private IPercentileSelector percentileSelector;
        private IChargesGenerator chargesGenerator;

        public WandGenerator(IPercentileSelector percentileSelector, IChargesGenerator chargesGenerator)
        {
            this.percentileSelector = percentileSelector;
            this.chargesGenerator = chargesGenerator;
        }

        public Item GenerateAtPower(string power)
        {
            var wand = new Item();
            wand.ItemType = ItemTypeConstants.Wand;
            wand.IsMagical = true;

            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, wand.ItemType);
            var spell = percentileSelector.SelectFrom(tablename);
            wand.Magic.Charges = chargesGenerator.GenerateFor(wand.ItemType, spell);
            wand.Name = string.Format("Wand of {0}", spell);
            wand.Attributes = new[] { AttributeConstants.Charged, AttributeConstants.OneTimeUse };

            return wand;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            template.Attributes = new[] { AttributeConstants.Charged, AttributeConstants.OneTimeUse };
            template.ItemType = ItemTypeConstants.Wand;

            var wand = template.Copy();
            wand.IsMagical = true;
            wand.Quantity = 1;

            return wand;
        }
    }
}