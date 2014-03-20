﻿using System;
using System.Collections.Generic;
using D20Dice;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using EquipmentGen.Generators.Interfaces.Items.Mundane;
using EquipmentGen.Generators.RuntimeFactories.Interfaces;
using EquipmentGen.Selectors.Interfaces;

namespace EquipmentGen.Generators.Items
{
    public class ItemsGenerator : IItemsGenerator
    {
        private ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private IMundaneItemGenerator mundaneItemGenerator;
        private IPercentileSelector percentileSelector;
        private IMagicalGearGeneratorFactory magicalGearGeneratorFactory;
        private IMagicalItemGeneratorFactory magicalItemGeneratorFactory;
        private ICurseGenerator curseGenerator;
        private IDice dice;

        public ItemsGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
            IMundaneItemGenerator mundaneItemGenerator, IPercentileSelector percentileSelector,
            IMagicalGearGeneratorFactory magicalGearGeneratorFactory, IMagicalItemGeneratorFactory magicalItemGeneratorFactory,
            ICurseGenerator curseGenerator, IDice dice)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.mundaneItemGenerator = mundaneItemGenerator;
            this.percentileSelector = percentileSelector;
            this.magicalGearGeneratorFactory = magicalGearGeneratorFactory;
            this.magicalItemGeneratorFactory = magicalItemGeneratorFactory;
            this.curseGenerator = curseGenerator;
            this.dice = dice;
        }

        public IEnumerable<Item> GenerateAtLevel(Int32 level)
        {
            var roll = dice.Percentile();
            var tableName = String.Format("Level{0}Items", level);
            var typeAndAmountResult = typeAndAmountPercentileSelector.SelectFrom(tableName, roll);
            var items = new List<Item>();

            if (String.IsNullOrEmpty(typeAndAmountResult.Type))
                return items;

            var amount = dice.Roll(typeAndAmountResult.AmountToRoll);

            while (amount-- > 0)
            {
                var item = GenerateAtPower(typeAndAmountResult.Type);
                items.Add(item);
            }

            return items;
        }

        public Item GenerateAtPower(String power)
        {
            if (power == PowerConstants.Mundane)
                return mundaneItemGenerator.Generate();

            return GenerateMagicalItemAtPower(power);
        }

        private Item GenerateMagicalItemAtPower(String power)
        {
            var roll = dice.Percentile();
            var tableName = String.Format("{0}Items", power);
            var type = percentileSelector.SelectFrom(tableName, roll);

            Item item;
            if (type == ItemTypeConstants.Armor || type == ItemTypeConstants.Weapon)
                item = GenerateMagicalGearAtPower(type, power);
            else
                item = GenerateMagicalItemAtPower(type, power);

            if (curseGenerator.HasCurse(item.Magic))
            {
                var curse = curseGenerator.GenerateCurse();
                if (curse == "SpecificCursedItem")
                    return curseGenerator.GenerateSpecificCursedItem();

                item.Magic.Add(Magic.Curse, curse);
            }

            return item;
        }

        private Item GenerateMagicalGearAtPower(String type, String power)
        {
            var powerGearGenerator = magicalGearGeneratorFactory.CreateWith(type);
            return powerGearGenerator.GenerateAtPower(power);
        }

        private Item GenerateMagicalItemAtPower(String type, String power)
        {
            var magicalItemGenerator = magicalItemGeneratorFactory.CreateWith(type);
            return magicalItemGenerator.GenerateAtPower(power);
        }
    }
}