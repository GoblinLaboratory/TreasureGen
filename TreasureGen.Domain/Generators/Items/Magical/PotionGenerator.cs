﻿using System;
using System.Collections.Generic;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class PotionGenerator : MagicalItemGenerator
    {
        private ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private IPercentileSelector percentileSelector;

        public PotionGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector, IPercentileSelector percentileSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.percentileSelector = percentileSelector;
        }

        public Item GenerateAtPower(string power)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);
            var potion = new Item();

            potion.Name = result.Type;
            potion.BaseNames = new[] { result.Type };
            potion.ItemType = ItemTypeConstants.Potion;
            potion.Magic.Bonus = result.Amount;
            potion.IsMagical = true;
            potion.Attributes = new[] { AttributeConstants.OneTimeUse };

            return potion;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var potion = template.Clone();
            potion.BaseNames = new[] { potion.Name };
            potion.ItemType = ItemTypeConstants.Potion;
            potion.Attributes = new[] { AttributeConstants.OneTimeUse };
            potion.IsMagical = true;
            potion.Quantity = 1;

            return potion.SmartClone();
        }

        public Item GenerateFromSubset(string power, IEnumerable<string> subset)
        {
            throw new NotImplementedException();
        }
    }
}