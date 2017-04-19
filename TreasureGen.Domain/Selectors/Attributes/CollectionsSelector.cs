﻿using RollGen;
using System;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Mappers.Collections;

namespace TreasureGen.Domain.Selectors.Attributes
{
    internal class CollectionsSelector : ICollectionsSelector
    {
        private ICollectionsMapper attributesMapper;
        private Dice dice;

        public CollectionsSelector(ICollectionsMapper attributesMapper)
        {
            this.attributesMapper = attributesMapper;
        }

        public bool Exists(string tableName, string name)
        {
            var table = attributesMapper.Map(tableName);
            return table.ContainsKey(name);
        }

        public IEnumerable<string> SelectFrom(string tableName, string name)
        {
            if (!Exists(tableName, name))
            {
                var message = string.Format("{0} is not in the table {1}", name, tableName);
                throw new ArgumentException(message);
            }

            var table = attributesMapper.Map(tableName);
            return table[name];
        }

        public string SelectRandomFrom(IEnumerable<string> collection)
        {
            if (!collection.Any())
                throw new ArgumentException("Cannot select random from an empty collection");

            var index = dice.Roll(1).d(collection.Count()).AsSum() - 1;
            return collection.ElementAt(index);
        }
    }
}