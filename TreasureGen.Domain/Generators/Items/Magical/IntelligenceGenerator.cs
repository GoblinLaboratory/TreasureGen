﻿using RollGen;
using System;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class IntelligenceGenerator : IIntelligenceGenerator
    {
        private Dice dice;
        private IPercentileSelector percentileSelector;
        private ICollectionsSelector attributesSelector;
        private IIntelligenceAttributesSelector intelligenceAttributesSelector;
        private IBooleanPercentileSelector booleanPercentileSelector;

        public IntelligenceGenerator(Dice dice, IPercentileSelector percentileSelector, ICollectionsSelector attributesSelector,
            IIntelligenceAttributesSelector intelligenceAttributesSelector, IBooleanPercentileSelector booleanPercentileSelector)
        {
            this.dice = dice;
            this.percentileSelector = percentileSelector;
            this.attributesSelector = attributesSelector;
            this.intelligenceAttributesSelector = intelligenceAttributesSelector;
            this.booleanPercentileSelector = booleanPercentileSelector;
        }

        public bool IsIntelligent(string itemType, IEnumerable<string> attributes, bool isMagical)
        {
            if (!isMagical)
                return false;

            if (attributes.Contains(AttributeConstants.OneTimeUse) || attributes.Contains(AttributeConstants.Ammunition))
                return false;

            if (attributes.Contains(AttributeConstants.Melee))
                itemType = AttributeConstants.Melee;
            else if (attributes.Contains(AttributeConstants.Ranged))
                itemType = AttributeConstants.Ranged;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            return booleanPercentileSelector.SelectFrom(tableName);
        }

        public Intelligence GenerateFor(Item item)
        {
            var highStatResult = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats);
            var highStat = Convert.ToInt32(highStatResult);

            var intelligence = new Intelligence();
            intelligence.Ego += highStat - 10 - highStat % 2;
            intelligence.Ego += item.Magic.Bonus;

            foreach (var ability in item.Magic.SpecialAbilities)
                intelligence.Ego += ability.BonusEquivalent;

            switch (dice.Roll().d3().AsSum())
            {
                case 1: intelligence.CharismaStat = 10; break;
                case 2: intelligence.IntelligenceStat = 10; break;
                case 3: intelligence.WisdomStat = 10; break;
            }

            intelligence.CharismaStat = SetHighStat(highStat, intelligence.CharismaStat);
            intelligence.IntelligenceStat = SetHighStat(highStat, intelligence.IntelligenceStat);
            intelligence.WisdomStat = SetHighStat(highStat, intelligence.WisdomStat);

            intelligence.Communication = attributesSelector.SelectFrom(TableNameConstants.Collections.Set.IntelligenceCommunication, highStatResult);

            if (intelligence.Communication.Contains("Speech"))
                intelligence.Languages = GenerateLanguages(intelligence.IntelligenceStat);

            intelligence.Ego += BoostEgoByCommunication(intelligence.Communication, "Read");
            intelligence.Ego += BoostEgoByCommunication(intelligence.Communication, "Read magic");
            intelligence.Ego += BoostEgoByCommunication(intelligence.Communication, "Telepathy");

            var intelligenceAttributesResult = intelligenceAttributesSelector.SelectFrom(TableNameConstants.Collections.Set.IntelligenceAttributes, highStatResult);
            intelligence.Senses = intelligenceAttributesResult.Senses;

            var lesserPowers = GeneratePowers("Lesser", intelligenceAttributesResult.LesserPowersCount);
            intelligence.Ego += lesserPowers.Count;
            intelligence.Powers.AddRange(lesserPowers);

            var greaterPowers = GeneratePowers("Greater", intelligenceAttributesResult.GreaterPowersCount);

            var roll = dice.Roll().d4().AsSum();
            if (roll <= intelligenceAttributesResult.GreaterPowersCount)
            {
                greaterPowers.RemoveAt(greaterPowers.Count - 1);
                intelligence.SpecialPurpose = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes);
                intelligence.DedicatedPower = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers);
                intelligence.Ego += 4;
            }

            intelligence.Ego += greaterPowers.Count * 2;
            intelligence.Powers.AddRange(greaterPowers);
            intelligence.Alignment = GetAlignment(item);
            intelligence.Personality = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.PersonalityTraits);

            return intelligence;
        }

        private int BoostEgoByCommunication(IEnumerable<string> communication, string communicationType)
        {
            var containsCommunicationType = communication.Contains(communicationType);
            return Convert.ToInt32(containsCommunicationType);
        }

        private int SetHighStat(int highStat, int stat)
        {
            if (stat == 0)
                return highStat;

            return stat;
        }

        private List<string> GenerateLanguages(int intelligenceStat)
        {
            var modifier = (intelligenceStat - 10) / 2;
            var languages = GetNonDuplicateList(TableNameConstants.Percentiles.Set.Languages, modifier);
            languages.Add("Common");

            return languages;
        }

        private List<string> GeneratePowers(string strength, int count)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, strength);
            return GetNonDuplicateList(tableName, count);
        }

        private List<string> GetNonDuplicateList(string tableName, int quantity)
        {
            var list = new List<string>();

            while (list.Count < quantity)
            {
                var result = percentileSelector.SelectFrom(tableName);

                if (result.Equals("Common", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (list.Contains(result))
                    continue;

                list.Add(result);
            }

            return list;
        }

        private string GetAlignment(Item item)
        {
            string alignment;
            var abilityNames = item.Magic.SpecialAbilities.Select(a => a.Name);
            var specificAlignmentRequirement = GetSpecificAlignmentRequirement(item);

            do alignment = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments);
            while (!AlignmentIsAllowed(alignment, abilityNames, specificAlignmentRequirement));

            return alignment;
        }

        private string GetSpecificAlignmentRequirement(Item item)
        {
            var itemsWithSpecificAlignments = attributesSelector.SelectFrom(TableNameConstants.Collections.Set.ItemAlignmentRequirements, "Items");

            if (itemsWithSpecificAlignments.Contains(item.Name))
                return attributesSelector.SelectFrom(TableNameConstants.Collections.Set.ItemAlignmentRequirements, item.Name).Single();

            var alignments = AlignmentConstants.GetAllAlignments();
            var requirements = alignments.Where(a => item.Traits.Any(t => t.Contains(a)));

            if (requirements.Any())
                //INFO: If there is more than 1 alignment requirement in the traits, then there is a problem
                return requirements.Single();

            alignments = AlignmentConstants.GetAllPartialAlignments();
            requirements = alignments.Where(a => item.Traits.Any(t => t.Contains(a)));

            if (requirements.Any())
                //INFO: If there is more than 1 partial alignment requirement in the traits, then there is a problem
                //i.e., they either conflict or should have been a full alignment requriement
                return requirements.Single();

            return string.Empty;
        }

        private bool AlignmentIsAllowed(string alignment, IEnumerable<string> abilityNames, string specificAlignmentRequirement)
        {
            if (alignment == AlignmentConstants.TrueNeutral)
                return true;

            if (abilityNames.Contains(SpecialAbilityConstants.Anarchic) && alignment.StartsWith(AlignmentConstants.Lawful))
                return false;

            if (abilityNames.Contains(SpecialAbilityConstants.Axiomatic) && alignment.StartsWith(AlignmentConstants.Chaotic))
                return false;

            if (abilityNames.Contains(SpecialAbilityConstants.Holy) && alignment.EndsWith(AlignmentConstants.Evil))
                return false;

            if (abilityNames.Contains(SpecialAbilityConstants.Unholy) && alignment.EndsWith(AlignmentConstants.Good))
                return false;

            return alignment == specificAlignmentRequirement || PartialAlignmentRequirementMet(alignment, specificAlignmentRequirement);
        }

        private bool PartialAlignmentRequirementMet(string alignment, string specificAlignmentRequirement)
        {
            if (specificAlignmentRequirement == AlignmentConstants.Neutral)
                return alignment.EndsWith(specificAlignmentRequirement);

            return alignment.StartsWith(specificAlignmentRequirement) || alignment.EndsWith(specificAlignmentRequirement);
        }
    }
}