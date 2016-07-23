﻿using RollGen;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items
{
    internal class SpecificGearGenerator : ISpecificGearGenerator
    {
        private ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private IAttributesSelector attributesSelector;
        private ISpecialAbilityAttributesSelector specialAbilityAttributesSelector;
        private IChargesGenerator chargesGenerator;
        private IPercentileSelector percentileSelector;
        private ISpellGenerator spellGenerator;
        private IBooleanPercentileSelector booleanPercentileSelector;
        private Dice dice;

        public SpecificGearGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector, IAttributesSelector attributesSelector, ISpecialAbilityAttributesSelector specialAbilityAttributesSelector, IChargesGenerator chargesGenerator, IPercentileSelector percentileSelector, ISpellGenerator spellGenerator, IBooleanPercentileSelector booleanPercentileSelector, Dice dice)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.attributesSelector = attributesSelector;
            this.specialAbilityAttributesSelector = specialAbilityAttributesSelector;
            this.chargesGenerator = chargesGenerator;
            this.percentileSelector = percentileSelector;
            this.spellGenerator = spellGenerator;
            this.booleanPercentileSelector = booleanPercentileSelector;
            this.dice = dice;
        }

        public Item GenerateFrom(string power, string specificGearType)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);

            var gear = new Item();
            gear.Name = result.Type;
            gear.Magic.Bonus = result.Amount;
            gear.Magic.SpecialAbilities = GetSpecialAbilities(specificGearType, gear.Name);

            if (specificGearType == ItemTypeConstants.Weapon)
                gear.ItemType = ItemTypeConstants.Weapon;
            else
                gear.ItemType = ItemTypeConstants.Armor;

            tableName = string.Format(TableNameConstants.Attributes.Formattable.SpecificITEMTYPEAttributes, specificGearType);
            gear.Attributes = attributesSelector.SelectFrom(tableName, gear.Name);

            tableName = string.Format(TableNameConstants.Attributes.Formattable.SpecificITEMTYPETraits, specificGearType);
            var traits = attributesSelector.SelectFrom(tableName, gear.Name);

            foreach (var trait in traits)
                gear.Traits.Add(trait);

            if (gear.Attributes.Contains(AttributeConstants.Charged))
                gear.Magic.Charges = chargesGenerator.GenerateFor(specificGearType, gear.Name);

            if (gear.Name == WeaponConstants.JavelinOfLightning)
            {
                gear.IsMagical = true;
            }
            else if (gear.Name == ArmorConstants.CastersShield)
            {
                var hasSpell = booleanPercentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.CastersShieldContainsSpell);

                if (hasSpell)
                {
                    var spellType = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.CastersShieldSpellTypes);
                    var spellLevel = spellGenerator.GenerateLevel(PowerConstants.Medium);
                    var spell = spellGenerator.Generate(spellType, spellLevel);
                    var formattedSpell = string.Format("{0} ({1}, {2})", spell, spellType, spellLevel);
                    gear.Contents.Add(formattedSpell);
                }
            }

            gear.Name = RenameGear(gear.Name);
            gear.Quantity = GetQuantity(gear);

            if (gear.Name == WeaponConstants.SlayingArrow || gear.Name == WeaponConstants.GreaterSlayingArrow)
            {
                var designatedFoe = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.DesignatedFoes);
                var trait = string.Format("Designated Foe: {0}", designatedFoe);
                gear.Traits.Add(trait);
            }

            if (gear.IsMagical == false)
            {
                var size = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes);
                gear.Traits.Add(size);
            }

            return gear;
        }

        private int GetQuantity(Item gear)
        {
            var thrownWeapons = attributesSelector.SelectFrom(TableNameConstants.Attributes.Set.AmmunitionAttributes, AttributeConstants.Thrown);

            if (gear.Attributes.Contains(AttributeConstants.Ammunition) == false && thrownWeapons.Contains(gear.Name) == false)
                return 1;

            return dice.Roll().d20();
        }

        private string RenameGear(string oldName)
        {
            switch (oldName)
            {
                case WeaponConstants.SilverDagger: return WeaponConstants.Dagger;
                case WeaponConstants.LuckBlade0: return WeaponConstants.LuckBlade;
                case WeaponConstants.LuckBlade1: return WeaponConstants.LuckBlade;
                case WeaponConstants.LuckBlade2: return WeaponConstants.LuckBlade;
                case WeaponConstants.LuckBlade3: return WeaponConstants.LuckBlade;
                default: return oldName;
            }
        }

        private IEnumerable<SpecialAbility> GetSpecialAbilities(string specificGearType, string name)
        {
            var tableName = string.Format(TableNameConstants.Attributes.Formattable.SpecificITEMTYPESpecialAbilities, specificGearType);
            var abilityNames = attributesSelector.SelectFrom(tableName, name);

            return abilityNames.Select(n => ReconstituteAbility(n));
        }

        private SpecialAbility ReconstituteAbility(string name)
        {
            var abilityResult = specialAbilityAttributesSelector.SelectFrom(TableNameConstants.Attributes.Set.SpecialAbilityAttributes, name);
            var ability = new SpecialAbility();

            ability.Name = name;
            ability.AttributeRequirements = attributesSelector.SelectFrom(TableNameConstants.Attributes.Set.SpecialAbilityAttributeRequirements, abilityResult.BaseName);
            ability.BaseName = abilityResult.BaseName;
            ability.BonusEquivalent = abilityResult.BonusEquivalent;
            ability.Power = abilityResult.Power;

            return ability;
        }
    }
}