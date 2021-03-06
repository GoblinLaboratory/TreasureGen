﻿using Moq;
using NUnit.Framework;
using RollGen;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Selectors.Results;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class SpecialAbilitiesGeneratorTests
    {
        private ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private Mock<ICollectionsSelector> mockCollectionsSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IBooleanPercentileSelector> mockBooleanPercentileSelector;
        private Mock<ISpecialAbilityAttributesSelector> mockSpecialAbilityAttributesSelector;
        private Mock<Dice> mockDice;

        private List<string> itemAttributes;
        private List<string> names;
        private string power;

        [SetUp]
        public void Setup()
        {
            itemAttributes = new List<string>();

            mockCollectionsSelector = new Mock<ICollectionsSelector>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockBooleanPercentileSelector = new Mock<IBooleanPercentileSelector>();
            mockSpecialAbilityAttributesSelector = new Mock<ISpecialAbilityAttributesSelector>();
            mockDice = new Mock<Dice>();
            names = new List<string>();
            power = "power";

            mockPercentileSelector.Setup(p => p.SelectAllFrom(It.IsAny<string>())).Returns(names);
            mockDice.Setup(d => d.Roll(1).d(It.IsAny<int>()).AsSum()).Returns(1);

            specialAbilitiesGenerator = new SpecialAbilitiesGenerator(mockCollectionsSelector.Object, mockPercentileSelector.Object,
                mockSpecialAbilityAttributesSelector.Object, mockBooleanPercentileSelector.Object, mockDice.Object);
        }

        [Test]
        public void ReturnEmptyIfBonusLessThanOne()
        {
            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 0, 1);
            Assert.That(abilities, Is.Empty);
        }

        [Test]
        public void GetShieldAbilityIfShield()
        {
            itemAttributes.Add(AttributeConstants.Shield);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Shield);

            specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(tableName), Times.Once);
        }

        [Test]
        public void GetMeleeAbilityIfMelee()
        {
            itemAttributes.Add(AttributeConstants.Melee);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Melee);

            specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Weapon, itemAttributes, power, 1, 1);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(tableName), Times.Once);
        }

        [Test]
        public void GetRangedAbilityIfRanged()
        {
            itemAttributes.Add(AttributeConstants.Ranged);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Ranged);

            specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Weapon, itemAttributes, power, 1, 1);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(tableName), Times.Once);
        }

        [Test]
        public void GetArmorAbilityIfArmor()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, ItemTypeConstants.Armor);
            specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(tableName), Times.Once);
        }

        [Test]
        public void ReturnEmptyIfNoMatchingTableNames()
        {
            var abilities = specialAbilitiesGenerator.GenerateFor("wrong item type", itemAttributes, power, 1, 1);
            Assert.That(abilities, Is.Empty);
        }

        [Test]
        public void SetAbilityByResult()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns("name");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAbilities()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 3);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 3"));
            Assert.That(names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void WeaponsThatAreBothMeleeAndRangedGetRandomlyBetweenTables()
        {
            itemAttributes.Add(AttributeConstants.Melee);
            itemAttributes.Add(AttributeConstants.Ranged);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Melee);
            mockPercentileSelector.Setup(p => p.SelectAllFrom(tableName)).Returns(new[] { "melee ability" });
            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Ranged);
            mockPercentileSelector.Setup(p => p.SelectAllFrom(tableName)).Returns(new[] { "ranged ability" });

            var meleeResult = new SpecialAbilityAttributesResult();
            meleeResult.BaseName = "melee ability";
            mockSpecialAbilityAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributes, meleeResult.BaseName)).Returns(meleeResult);
            mockCollectionsSelector.Setup(p => p.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, meleeResult.BaseName)).Returns(itemAttributes);

            var rangedResult = new SpecialAbilityAttributesResult();
            rangedResult.BaseName = "ranged ability";
            mockSpecialAbilityAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributes, rangedResult.BaseName)).Returns(rangedResult);
            mockCollectionsSelector.Setup(p => p.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, rangedResult.BaseName)).Returns(itemAttributes);

            mockDice.SetupSequence(d => d.Roll(1).d(2).AsSum()).Returns(1).Returns(2);

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 2);
            var names = abilities.Select(a => a.Name);

            Assert.That(names, Contains.Item("melee ability"));
            Assert.That(names, Contains.Item("ranged ability"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DoNotAllowAbilitiesAndMagicBonusToBeGreaterThan10()
        {
            CreateSpecialAbility("big ability", bonus: 2);
            CreateSpecialAbility("small ability", bonus: 1);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("big ability").Returns("small ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 9, 1);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("small ability"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void AccumulateAbilities()
        {
            CreateSpecialAbility("ability 1", bonus: 2);
            CreateSpecialAbility("ability 2", bonus: 2);
            CreateSpecialAbility("ability 3", bonus: 3);
            CreateSpecialAbility("ability 4", bonus: 0);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 5, 3);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void ReplaceWeakWithStrong()
        {
            CreateSpecialAbility("weak ability", "ability", power: 1);
            CreateSpecialAbility("strong ability", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("weak ability").Returns("strong ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 2);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("strong ability"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void DoNotCompareStrengthForDissimilarCoreName()
        {
            CreateSpecialAbility("weak ability", power: 1);
            CreateSpecialAbility("strong ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("weak ability").Returns("strong ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 2);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("weak ability"));
            Assert.That(names, Contains.Item("strong ability"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DoNotReplaceStrongWithWeak()
        {
            CreateSpecialAbility("strong ability", "ability", power: 2);
            CreateSpecialAbility("weak ability", "ability", power: 1);
            CreateSpecialAbility("other ability");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("strong ability").Returns("weak ability")
                .Returns("other ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 2);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("strong ability"));
            Assert.That(names, Is.Not.Contains("weak ability"));
        }

        [Test]
        public void AbilitiesMaxOutAtBonusOf10()
        {
            CreateSpecialAbility("ability 1", bonus: 2);
            CreateSpecialAbility("ability 2", bonus: 2);
            CreateSpecialAbility("ability 3", bonus: 3);
            CreateSpecialAbility("ability 4", bonus: 3);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 4);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 3"));
            Assert.That(names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CanGetAbilitiesWithBonusOf0WhileAtBonusOf10()
        {
            CreateSpecialAbility("ability 1", bonus: 9);
            CreateSpecialAbility("ability 2", bonus: 0);
            CreateSpecialAbility("ability 3", bonus: 3);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 2);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DuplicateAbilitiesCannotBeAdded()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 1")
                .Returns("ability 2");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 2);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void AbilitiesFilteredByAttributeRequirementsFromBaseName()
        {
            CreateSpecialAbility("ability 1", "base ability 1");
            CreateSpecialAbility("ability 2", "base ability 2");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2");

            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base ability 1")).Returns(new[] { "other type", "type 1" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base ability 2")).Returns(itemAttributes);

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void AbilitiesFilteredByEitherAttributeRequirementsFromBaseName()
        {
            itemAttributes.Add("or");

            CreateSpecialAbility("ability 1", "base ability 1");
            CreateSpecialAbility("ability 2", "base ability 2");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2");

            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base ability 1")).Returns(new[] { "other/order/of/type", "type 1" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base ability 2")).Returns(new[] { "either/or" });

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ExtraAttributesDoNotMatter()
        {
            CreateSpecialAbility("ability");
            mockPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns("ability");
            itemAttributes.Add("type 1");
            var inputTypes = itemAttributes.Union(new[] { "other type" });

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, inputTypes, power, 1, 1);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void BonusSpecialAbilitiesAdded()
        {
            CreateSpecialAbility("ability 1", "base ability 1");
            CreateSpecialAbility("ability 2", "base ability 2");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("BonusSpecialAbility").Returns("ability 1")
                .Returns("ability 2");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void StopIfAllPossibleAbilitiesAcquired()
        {
            CreateSpecialAbility("ability");
            mockPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns("ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 5);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CountSameCoreNameAsSameAbility()
        {
            CreateSpecialAbility("ability 1", "base name", power: 1);
            CreateSpecialAbility("ability 2", "base name", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 5);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ReturnEmptyIfNoCompatibleAbilities()
        {
            mockPercentileSelector.Setup(p => p.SelectAllFrom(It.IsAny<string>())).Returns(Enumerable.Empty<string>());
            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 1);
            Assert.That(abilities, Is.Empty);
        }

        [Test]
        public void ReturnAllAbilitiesWithStrongestIfQuantityGreaterThanOrEqualToAllAvailableAbilities()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3", "ability", power: 1);
            CreateSpecialAbility("ability 4", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 4);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DoNotReturnAbilitiesWithBonusSumGreaterThan10()
        {
            CreateSpecialAbility("ability 1", bonus: 3);
            CreateSpecialAbility("ability 2", bonus: 3);
            CreateSpecialAbility("ability 3", bonus: 3);
            CreateSpecialAbility("ability 4", bonus: 3);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 5);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 3"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public void RemoveWeakerAbilitiesFromAvailableWhenStrongAdded()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3", "ability", power: 1);
            CreateSpecialAbility("ability 4", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 4").Returns("ability 2")
                .Returns("ability 3").Returns("ability 1");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 3);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void WhenAddingAllAbilities_UpgradeAllWeakAbilities()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3", "ability", power: 1);
            CreateSpecialAbility("ability 4", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(It.IsAny<string>())).Returns("ability 3").Returns("BonusSpecialAbility")
                .Returns("ability 2").Returns("ability 1").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(ItemTypeConstants.Armor, itemAttributes, power, 1, 3);
            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(It.IsAny<string>()), Times.Exactly(2));
        }

        private void CreateSpecialAbility(string name, string baseName = "", int bonus = 0, int power = 0)
        {
            var result = new SpecialAbilityAttributesResult();

            if (string.IsNullOrEmpty(baseName))
                result.BaseName = name;
            else
                result.BaseName = baseName;

            result.BonusEquivalent = bonus;
            result.Power = power;
            names.Add(name);

            mockSpecialAbilityAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributes, name)).Returns(result);
            mockCollectionsSelector.Setup(p => p.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, result.BaseName)).Returns(itemAttributes);
        }

        [Test]
        public void CreateSetSpecialAbilities()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3", "base 3", 1337, 1234);

            var attributeRequirements = new List<IEnumerable<string>>();
            attributeRequirements.Add(new[] { "other type 1", "type 1" });
            attributeRequirements.Add(new[] { "other type 2", "type 2" });
            attributeRequirements.Add(new[] { "other type 3", "type 3" });

            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            var abilities = specialAbilitiesGenerator.GenerateFor(new[] { "ability 1", "ability 2", "ability 3" });

            var abilityList = abilities.ToList();
            Assert.That(abilityList[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityList[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityList[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityList[0].Power, Is.EqualTo(90210));
            Assert.That(abilityList[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityList[1].AttributeRequirements, Is.EqualTo(attributeRequirements[1]));
            Assert.That(abilityList[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityList[1].BaseName, Is.EqualTo("base 2"));
            Assert.That(abilityList[1].Power, Is.EqualTo(600));
            Assert.That(abilityList[1].BonusEquivalent, Is.EqualTo(42));
            Assert.That(abilityList[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityList[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityList[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityList[2].Power, Is.EqualTo(1234));
            Assert.That(abilityList[2].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityList.Count, Is.EqualTo(3));
        }
    }
}