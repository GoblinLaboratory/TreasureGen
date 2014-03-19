﻿using System;
using D20Dice;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Mundane;
using EquipmentGen.Generators.RuntimeFactories.Interfaces;
using EquipmentGen.Selectors.Interfaces;

namespace EquipmentGen.Generators.Items.Mundane
{
    public class MundaneItemGenerator : IMundaneItemGenerator
    {
        private IPercentileSelector percentileResultProvider;
        private IAlchemicalItemGenerator alchemicalItemFactory;
        private IMundaneGearGeneratorFactory gearGeneratorFactory;
        private IToolGenerator toolGenerator;
        private IDice dice;

        public MundaneItemGenerator(IPercentileSelector percentileResultProvider, IAlchemicalItemGenerator alchemicalItemFactory,
            IMundaneGearGeneratorFactory gearGeneratorFactory, IToolGenerator toolGenerator, IDice dice)
        {
            this.percentileResultProvider = percentileResultProvider;
            this.alchemicalItemFactory = alchemicalItemFactory;
            this.gearGeneratorFactory = gearGeneratorFactory;
            this.toolGenerator = toolGenerator;
            this.dice = dice;
        }

        public Item Generate()
        {
            var roll = dice.Percentile();
            var type = percentileResultProvider.SelectFrom("MundaneItems", roll);
            return GetItem(type);
        }

        private Item GetItem(String type)
        {
            switch (type)
            {
                case ItemTypeConstants.AlchemicalItem: return alchemicalItemFactory.Generate();
                case ItemTypeConstants.Armor:
                case ItemTypeConstants.Weapon: return GenerateGear(type);
                case ItemTypeConstants.Tool: return toolGenerator.Generate();
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private Item GenerateGear(String type)
        {
            var generator = gearGeneratorFactory.CreateWith(type);
            return generator.Generate();
        }
    }
}