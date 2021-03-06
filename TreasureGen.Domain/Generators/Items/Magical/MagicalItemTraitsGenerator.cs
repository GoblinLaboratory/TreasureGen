﻿using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class MagicalItemTraitsGenerator : IMagicalItemTraitsGenerator
    {
        private IPercentileSelector percentileSelector;

        public MagicalItemTraitsGenerator(IPercentileSelector percentileSelector)
        {
            this.percentileSelector = percentileSelector;
        }

        public IEnumerable<string> GenerateFor(string itemType, IEnumerable<string> attributes)
        {
            var tableName = GetTableName(itemType, attributes);
            var result = percentileSelector.SelectFrom(tableName);

            if (string.IsNullOrEmpty(result))
                return Enumerable.Empty<string>();

            return result.Split(',');
        }

        private string GetTableName(string itemType, IEnumerable<string> attributes)
        {
            if (attributes.Contains(AttributeConstants.Melee))
                return string.Format(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, AttributeConstants.Melee);

            if (attributes.Contains(AttributeConstants.Ranged))
                return string.Format(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, AttributeConstants.Ranged);

            return string.Format(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, itemType);
        }
    }
}