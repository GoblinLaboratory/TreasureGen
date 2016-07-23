﻿using Ninject;
using NUnit.Framework;
using TreasureGen.Items;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Tests.Integration.Stress.Items.Mundane
{
    [TestFixture]
    public class MundaneWeaponGeneratorTests : MundaneItemGeneratorStressTests
    {
        [Inject, Named(ItemTypeConstants.Weapon)]
        public MundaneItemGenerator MundaneWeaponGenerator { get; set; }

        [Test]
        public void StressWeapon()
        {
            Stress(StressItem);
        }

        protected override void MakeSpecificAssertionsAgainst(Item weapon)
        {
            Assert.That(weapon.Name, Is.Not.Empty);
            Assert.That(weapon.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(weapon.Quantity, Is.GreaterThan(0));
            Assert.That(weapon.IsMagical, Is.False);
            Assert.That(weapon.Attributes, Contains.Item(AttributeConstants.Common)
                .Or.Contains(AttributeConstants.Uncommon));
            Assert.That(weapon.Attributes, Contains.Item(AttributeConstants.Melee)
                .Or.Contains(AttributeConstants.Ranged));
            Assert.That(weapon.Contents, Is.Empty);
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Small)
                .Or.Contains(TraitConstants.Medium)
                .Or.Contains(TraitConstants.Large));
        }

        protected override Item GenerateItem()
        {
            return MundaneWeaponGenerator.Generate();
        }

        [Test]
        public override void SpecialMaterialsHappen()
        {
            base.SpecialMaterialsHappen();
        }

        [Test]
        public void MasterworkHappens()
        {
            var weapon = GenerateOrFail(GenerateItem, w => w.Traits.Contains(TraitConstants.Masterwork));
            AssertItem(weapon);
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public override void NoDecorationsHappen()
        {
            AssertNoDecorationsHappen();
        }
    }
}