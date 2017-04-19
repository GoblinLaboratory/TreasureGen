﻿using System;
using System.Collections.Generic;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Domain.Generators.Items.Mundane
{
    internal class AlchemicalItemGenerator : MundaneItemGenerator
    {
        private ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;

        public AlchemicalItemGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
        }

        public Item Generate()
        {
            var result = typeAndAmountPercentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.AlchemicalItems);

            var item = new Item();
            item.Name = result.Type;
            item.Quantity = result.Amount;
            item.ItemType = ItemTypeConstants.AlchemicalItem;
            item.BaseNames = new[] { item.Name };

            return item;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = template.MundaneClone();
            item.ItemType = ItemTypeConstants.AlchemicalItem;
            item.BaseNames = new[] { item.Name };

            return item;
        }

        public Item GenerateFromSubset(IEnumerable<string> subset)
        {
            throw new NotImplementedException();
        }
    }
}